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

            var postInternal = PostReader.GetPost(postID);
            if (postInternal == null)
            {
                return NotFound();
            }
            var parentID = postInternal.ParentID;
            bool IsUserBanned = true;
            bool IsUserMember = false;
            SchoolClub postClub = null;
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            TaskList tasks = new TaskList();
            tasks.Add(() => { postClub = SchoolClubReader.GetClub(parentID); });
            tasks.Add(() => { IsUserBanned = SchoolClubReader.IsUserBanned(parentID, cmsUser.ID.Value); });
            tasks.Add(() => { IsUserMember = SchoolClubReader.ValidateMembership(parentID, cmsUser.ID.Value); });
            tasks.ExecuteAll();




            var postPublic = postInternal.ToDto<Post_R_PublicDTO>();

            if (postClub != null)
            {
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
                    return Ok(postPublic);
                }
                else
                {
                    if (postInternal.IsPublic)
                    {
                        return Ok(postPublic);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Forbidden, "Access Denied");
                    }
                }
            }


            // This is what happens if the parent is a school.
            //verify same school
            if (postInternal.ParentID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            return Ok(postPublic);
        }
    }
}
