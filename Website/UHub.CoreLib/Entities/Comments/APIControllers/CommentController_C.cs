using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Comments.DTOs;
using UHub.CoreLib.Entities.Comments.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.Comments.Management;

namespace UHub.CoreLib.Entities.Comments.APIControllers
{
    
    public sealed partial class CommentController
    {
        [HttpPost()]
        [Route("Create")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> Create([FromBody] Comment_C_PublicDTO Comment)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            if (Comment == null)
            {
                return BadRequest();
            }


            var tmpComment = Comment.ToInternal<Comment>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var validateParent = await UserReader.TryValidateCommentParentAsync((long)cmsUser.ID, tmpComment.ParentID);

            if (!validateParent)
            {
                status = "User is forbidden.";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            status = "Failed to create comment.";
            statCode = HttpStatusCode.BadRequest;

            try
            {
                tmpComment.CreatedBy = cmsUser.ID.Value;

                var CommentResult = await CommentManager.TryCreateCommentAsync(tmpComment);
                long? PostID = CommentResult.CommentID;
                var ResultCode = CommentResult.ResultCode;


                if (ResultCode == 0)
                {
                    status = PostID.ToString();
                    statCode = HttpStatusCode.OK;
                }
                else if(ResultCode == CommentResultCode.UnknownError)
                {
                    status = "Unknown server error";
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
                var errCode = "8b9255a4-070b-427c-91a9-4755199aaded";
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(errCode, ex);

                statCode = HttpStatusCode.InternalServerError;
            }


            return Content(statCode, status);

        }

    }
}
