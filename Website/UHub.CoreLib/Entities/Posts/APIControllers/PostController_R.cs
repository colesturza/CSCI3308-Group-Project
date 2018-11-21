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
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Security;

namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("GetByID")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByID(long postID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var postInternal = await PostReader.GetPostAsync(postID);
            if (postInternal == null)
            {
                return NotFound();
            }

            var parentID = postInternal.ParentID;
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();



            var taskPostClub = SchoolClubReader.GetClubAsync(parentID);
            var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(parentID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(parentID, cmsUser.ID.Value);



            var postPublic = postInternal.ToDto<Post_R_PublicDTO>();

            var postClub = await taskPostClub;
            if (postClub != null)
            {
                //verify same school
                if (postClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var IsUserBanned = await taskIsUserBanned;
                //ensure not banned
                if (IsUserBanned)
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }

                var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
                if ((sanitizerMode & HtmlSanitizerMode.OnRead) != 0)
                {
                    postPublic.Content = postPublic.Content.SanitizeHtml();
                }


                var IsUserMember = await taskIsUserMember;
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
