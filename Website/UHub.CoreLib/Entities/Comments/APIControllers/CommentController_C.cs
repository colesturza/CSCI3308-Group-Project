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
using UHub.CoreLib.Entities.Comments.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;


namespace UHub.CoreLib.Entities.Comments.APIControllers
{
    
    public sealed partial class CommentController
    {
        [HttpPost()]
        [Route("Create")]
        [ApiAuthControl]
        public IHttpActionResult CreateComment([FromBody] Comment_C_PublicDTO comment)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }
            if (!HandleRecaptcha(out status))
            {
                return Content(statCode, status);
            }

            var tmpComment = comment.ToInternal<Comment>();

            var tmpUser = CoreFactory.Singleton.Auth.GetCurrentUser();



            if (!UserReader.ValidateCommentParent((long)tmpUser.ID, tmpComment.ParentID))
            {
                status = "User is forbidden.";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            status = "Failed to create comment.";
            statCode = HttpStatusCode.BadRequest;

            try
            {
                tmpComment.Content = tmpComment.Content.SanitizeHtml();

                long? PostID = CommentWriter.TryCreateComment(tmpComment);

                if (PostID != null)
                {
                    status = "Comment created.";
                    statCode = HttpStatusCode.OK;
                }

            }
            catch (Exception ex)
            {
                var errCode = "8b9255a4-070b-427c-91a9-4755199aaded";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }


            return Content(statCode, status);

        }

    }
}
