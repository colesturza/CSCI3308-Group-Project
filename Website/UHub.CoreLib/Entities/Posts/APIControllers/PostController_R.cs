using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.Management;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("GetByID")]
        [ApiAuthControl]
        public IHttpActionResult GetByID(long postID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var post = PostReader.GetPost(postID);
            var parentID = post.ParentID;

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var postClub = SchoolClubReader.GetClub(parentID);

            var outSet = post.ToDto<Post_R_PublicDTO>();

            if (postClub != null)
            {
                bool IsUserBanned = true;
                bool IsUserMember = false;

                TaskList tasks = new TaskList();
                tasks.Add(() => { IsUserBanned = SchoolClubReader.IsUserBanned(parentID, cmsUser.ID.Value); });
                tasks.Add(() => { IsUserMember = SchoolClubReader.ValidateMembership(parentID, cmsUser.ID.Value); });
                tasks.ExecuteAll();

                //verify same school
                if (postClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                //ensure not banned
                if (IsUserBanned)
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }

                //check for member status
                if (IsUserMember)
                {
                    return Ok(outSet);
                }
                else
                {
                    if (post.IsPublic)
                    {
                        return Ok(outSet);
                    }
                }
            }
            // This is what happens if the parent is a school.

            //verify same school
            if (post.ParentID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            return Ok(outSet);
        }
    }
}
