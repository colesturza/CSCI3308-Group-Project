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
using UHub.CoreLib.Entities.Posts.Management;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;


namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("Create")]
        [ApiAuthControl]
        public IHttpActionResult Create([FromBody] Post_C_PublicDTO post)
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

            if(post == null)
            {
                return BadRequest();
            }


            var tmpPost = post.ToInternal<Post>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            bool isValidParent = false;
            bool isUserBanned = true;

            TaskList tasks = new TaskList();
            tasks.Add(() => { isValidParent = UserReader.ValidatePostParent((long)cmsUser.ID, tmpPost.ParentID); });
            tasks.Add(() => { isUserBanned = SchoolClubReader.IsUserBanned(post.ParentID, cmsUser.ID.Value); });
            tasks.ExecuteAll();


            if (!isValidParent)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            if (isUserBanned)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }


            status = "Failed to create post.";
            statCode = HttpStatusCode.BadRequest;

            try
            {
                tmpPost.Content = tmpPost.Content.SanitizeHtml();
                tmpPost.CreatedBy = cmsUser.ID.Value;


                long? PostID = PostWriter.TryCreatePost(tmpPost);

                if (PostID != null)
                {
                    status = "Post created.";
                    statCode = HttpStatusCode.OK;
                }

            }
            catch (Exception ex)
            {
                var errCode = "d4bcfc43-5247-45a3-b448-5baeea96058e";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }


            return Content(statCode, status);

        }


    }
}
