using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.Posts.Management;

namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    public sealed partial class PostController
    {

        [HttpPost()]
        [Route("Update")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> Update([FromBody] Post_U_PublicDTO PostDTO)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            if (PostDTO == null)
            {
                return BadRequest();
            }
            if (PostDTO.ID == null)
            {
                return BadRequest();
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var PostInternal = await PostReader.TryGetPostAsync(PostDTO.ID.Value);

            if (PostInternal == null)
            {
                return BadRequest();
            }

            if (PostInternal.CreatedBy != cmsUser.ID.Value)
            {
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            }


            var taskIsValidParent = UserReader.TryValidatePostParentAsync((long)cmsUser.ID, PostInternal.ParentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(PostInternal.ParentID, cmsUser.ID.Value);


            var isValidParent = await taskIsValidParent;

            if (!isValidParent)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            if (await taskIsUserBanned)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }


            status = "Failed to Create Post.";
            statCode = HttpStatusCode.BadRequest;

            try
            {

                PostInternal.Name = PostDTO.Name;
                PostInternal.Content = PostDTO.Content;
                PostInternal.CanComment = PostDTO.CanComment;
                PostInternal.IsPublic = PostDTO.IsPublic;

                PostInternal.ModifiedBy = cmsUser.ID.Value;



                var postResult = await PostManager.TryUpdatePostAsync(PostInternal);
                var ResultCode = postResult;


                if (ResultCode == 0)
                {
                    status = "Post Updated";
                    statCode = HttpStatusCode.OK;
                }
                else if (ResultCode == PostResultCode.UnknownError)
                {
                    status = "Unknown Server Error";
                    statCode = HttpStatusCode.InternalServerError;
                }
                else
                {
                    status = "Invalid Field - " + ResultCode.ToString();
                    statCode = HttpStatusCode.BadRequest;
                }

            }
            catch (Exception ex)
            {
                var errCode = "2EAA1B03-A569-4A16-A77F-403F7DB7CA36";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                statCode = HttpStatusCode.InternalServerError;
            }


            return Content(statCode, status);

        }

    }
}
