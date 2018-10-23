using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/post")]
    public sealed class PostController : APIController
    {
        private protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }

            return true;
        }

        [HttpPost()]
        [Route("CreatePost")]
        public IHttpActionResult CreatePost([FromBody] Post_C_PublicDTO post)
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



            var tmpPost = post.ToInternal<Post>();

            var tmpUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            if (tmpUser == null || tmpUser.ID == null)
            {
                status = "User not authenticated.";
                statCode = HttpStatusCode.Unauthorized;
                return Content(statCode, status);
            }

            if (!UserReader.ValidatePostParent((long)tmpUser.ID, tmpPost.ParentID))
            {
                status = "User is forbidden.";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            status = "Failed to create post.";
            statCode = HttpStatusCode.BadRequest;

            try
            {
                tmpPost.Content = tmpPost.Content.HtmlEncode();

                long? PostID = PostWriter.TryCreatePost(tmpPost, tmpPost.ParentID);

                if (PostID != null)
                {
                    status = "Post created.";
                    statCode = HttpStatusCode.OK;
                }

            }
            catch (Exception ex)
            {
                var errCode = "dbdb7b89-af35-488a-9343-eaba09d62d41";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }


            return Content(statCode, status);

        }
    }
}
