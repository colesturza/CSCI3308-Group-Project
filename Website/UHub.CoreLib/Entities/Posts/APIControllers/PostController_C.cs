﻿using System;
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


namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("Create")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> Create([FromBody] Post_C_PublicDTO post)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            if (post == null)
            {
                return BadRequest();
            }


            var tmpPost = post.ToInternal<Post>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();



            var taskIsValidParent = UserReader.ValidatePostParentAsync((long)cmsUser.ID, tmpPost.ParentID);
            var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(post.ParentID, cmsUser.ID.Value);



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


            status = "Failed to create post.";
            statCode = HttpStatusCode.BadRequest;

            try
            {
                tmpPost.Name = tmpPost.Name.Trim();
                tmpPost.Content = tmpPost.Content.Trim();

                var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
                if ((sanitizerMode & HtmlSanitizerMode.OnWrite) != 0)
                {
                    tmpPost.Content = tmpPost.Content.SanitizeHtml();
                }
                tmpPost.CreatedBy = cmsUser.ID.Value;


                long? PostID = await PostWriter.TryCreatePostAsync(tmpPost);

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
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }


            return Content(statCode, status);

        }


    }
}
