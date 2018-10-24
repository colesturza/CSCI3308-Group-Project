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
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/posts")]
    public sealed class PostController : APIController
    {
        private const short DEFAULT_PAGE_SIZE = 20;


        private protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }
            return true;
        }

        [HttpPost()]
        [Route("GetPostCountBySchool")]
        [ApiAuthControl]
        public IHttpActionResult GetPostCountBySchool()
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var schoolID = cmsUser.SchoolID.Value;


            var count = PostReader.GetPostCountBySchool(schoolID);
            return Ok(count);

        }

        [HttpPost()]
        [Route("GetAllBySchool")]
        [ApiAuthControl]
        public IHttpActionResult GetAllBySchool()
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var schoolID = cmsUser.SchoolID.Value;


            var posts = PostReader.GetPostsBySchool(schoolID);

            var outSet = posts.Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();
            return Ok(outSet);

        }

        [HttpPost()]
        [Route("GetAllBySchool")]
        [ApiAuthControl]
        public IHttpActionResult GetPageBySchool(long? StartID = null, int? PageNum = null, short PageSize = DEFAULT_PAGE_SIZE)
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var schoolID = cmsUser.SchoolID.Value;


            var posts = PostReader.GetPostsBySchoolPage(schoolID, StartID, PageNum, PageSize);

            var outSet = posts.Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();
            return Ok(outSet);

        }


        [HttpPost()]
        [Route("GetPostCountByClub")]
        [ApiAuthControl]
        public IHttpActionResult GetPostCountByClub(long ClubID)
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var targetClub = SchoolClubReader.GetClub(ClubID);

            if (targetClub == null)
            {
                return NotFound();
            }

            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }


            var count = PostReader.GetPostCountByClub(ClubID);
            return Ok(count);
        }


        [HttpPost()]
        [Route("GetPostCountByClub")]
        [ApiAuthControl]
        public IHttpActionResult GetPageCountByClub(long ClubID, short PageSize)
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var targetClub = SchoolClubReader.GetClub(ClubID);

            if (targetClub == null)
            {
                return NotFound();
            }

            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }


            var count = PostReader.GetPostCountByClub(ClubID);
            if (PageSize == -1)
            {
                return Ok(1);
            }
            if (PageSize < 1)
            {
                double divResult = (count * 1.0) / DEFAULT_PAGE_SIZE;
                int ceil = (int)Math.Ceiling(divResult);
                return Ok(ceil);
            }


            double divResult2 = (count * 1.0) / PageSize;
            int ceil2 = (int)Math.Ceiling(divResult2);
            return Ok(ceil2);
        }


        [HttpPost()]
        [Route("GetAllByClub")]
        [ApiAuthControl]
        public IHttpActionResult GetAllByClub(long ClubID)
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            SchoolClub targetClub = null;
            bool IsUserBanned = true;
            bool IsUserMember = false;

            TaskList tasks = new TaskList();
            tasks.Add(() => { targetClub = SchoolClubReader.GetClub(ClubID); });
            tasks.Add(() => { IsUserBanned = SchoolClubReader.IsUserBanned(ClubID, cmsUser.ID.Value); });
            tasks.Add(() => { IsUserMember = SchoolClubReader.ValidateMembership(ClubID, cmsUser.ID.Value); });
            tasks.ExecuteAll();

            //verify same school
            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            //ensure not banned
            if (IsUserBanned)
            {
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            }


            var posts = PostReader.GetPostsByClub(ClubID);
            //check for member status
            if (IsUserMember)
            {
                var outSet = posts.Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();
                return Ok(outSet);
            }
            else
            {
                var outSet = posts.Where(x => x.IsPublic).Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();
                return Ok(outSet);
            }

        }


        [HttpPost()]
        [Route("GetPageByClub")]
        [ApiAuthControl]
        public IHttpActionResult GetPageByClub(long ClubID, long? StartID = null, int? PageNum = null, short PageSize = DEFAULT_PAGE_SIZE)
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            SchoolClub targetClub = null;
            bool IsUserBanned = true;
            bool IsUserMember = false;

            TaskList tasks = new TaskList();
            tasks.Add(() => { targetClub = SchoolClubReader.GetClub(ClubID); });
            tasks.Add(() => { IsUserBanned = SchoolClubReader.IsUserBanned(ClubID, cmsUser.ID.Value); });
            tasks.Add(() => { IsUserMember = SchoolClubReader.ValidateMembership(ClubID, cmsUser.ID.Value); });
            tasks.ExecuteAll();

            //verify same school
            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            //ensure not banned
            if (IsUserBanned)
            {
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            }


            var posts = PostReader.GetPostsByClubPage(ClubID, StartID, PageNum, PageSize);
            //check for member status
            if (IsUserMember)
            {
                var outSet = posts.Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();
                return Ok(outSet);
            }
            else
            {
                var outSet = posts.Where(x => x.IsPublic).Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();
                return Ok(outSet);
            }

        }



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
