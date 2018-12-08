using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Comments.DTOs;
using UHub.CoreLib.Entities.Comments.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Entities.Users.DTOs;

namespace UHub.CoreLib.Entities.Comments.APIControllers
{
    public sealed partial class CommentController
    {
        [HttpPost()]
        [Route("GetByPost")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByPost(long PostID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }




            var postInternal = await PostReader.TryGetPostAsync(PostID);
            if (postInternal == null)
            {
                return NotFound();
            }

            var parentID = postInternal.ParentID;
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var taskPostClub = SchoolClubReader.TryGetClubAsync(parentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(parentID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(parentID, cmsUser.ID.Value);
            var taskComments = CommentReader.TryGetCommentsByPostAsync(PostID);
            var taskUsers = UserReader.GetAllBySchoolAsync(cmsUser.SchoolID.Value);



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


                var IsUserMember = await taskIsUserMember;
                //check for member status
                if (IsUserMember || postInternal.IsPublic)
                {

                    await Task.WhenAll(taskComments, taskUsers);
                    var commentSet = taskComments.Result.Select(x => x.ToDto<Comment_R_PublicDTO>());
                    var userSet = taskUsers.Result.Select(x => x.ToDto<User_R_PublicDTO>());

                    //---------------------------------------------
                    //FOR COLE
                    //
                    //join user IDs to comment set to get userName
                    //use c# linq "join: tool
                    //
                    //look up "c# join query syntax"
                    //also
                    //"create new lambda class instance"
                    //---------------------------------------------


                    return Ok(commentSet);

                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }
            }


            // This is what happens if the parent is a school.
            //verify same school
            if (postInternal.ParentID != cmsUser.SchoolID)
            {
                return NotFound();
            }



            await Task.WhenAll(taskComments, taskUsers);
            var commentSetOuter = taskComments.Result.Select(x => x.ToDto<Comment_R_PublicDTO>());
            var userSetOuter = taskUsers.Result.Select(x => x.ToDto<User_R_PublicDTO>());

            //------------------------------------
            //FOR COLE
            //
            //repeat previous join statement here
            //------------------------------------


            return Ok(commentSetOuter);

        }
    }
}
