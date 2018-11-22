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

namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("GetPostCountByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPostCountByClub(long ClubID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var taskGetTargetClub = SchoolClubReader.GetClubAsync(ClubID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(ClubID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ClubID, cmsUser.ID.Value);



            var targetClub = await taskGetTargetClub;

            if (targetClub == null)
            {
                return NotFound();
            }

            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            var isUserMember = await taskIsUserMember;
            var taskGetCount = PostReader.GetPostCountByClubAsync(ClubID, isUserMember);


            if (await taskIsUserBanned)
            {
                return Ok(0);
            }


            var count = await taskGetCount;
            return Ok(count);
        }


        [HttpPost()]
        [Route("GetPageCountByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageCountByClub(long ClubID, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var taskGetTargetClub = SchoolClubReader.GetClubAsync(ClubID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;
            var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ClubID, cmsUser.ID.Value);


            var targetClub = await taskGetTargetClub;
            if (targetClub == null)
            {
                return NotFound();
            }

            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            var isUserMember = await taskIsUserMember;
            var taskGetCount = PostReader.GetPostCountByClubAsync(ClubID, isUserMember);


            var count = await taskGetCount;
            if (count == 0)
            {
                return Ok(0);
            }
            if (PageSize == -1)
            {
                return Ok(1);
            }
            if (PageSize < 1)
            {
                double divResult = (count * 1.0) / DEFAULT_PAGE_SIZE;
                long ceil = (long)Math.Ceiling(divResult);
                return Ok(ceil);
            }


            double divResult2 = (count * 1.0) / PageSize;
            long ceil2 = (long)Math.Ceiling(divResult2);
            return Ok(ceil2);
        }


        [HttpPost()]
        [Route("GetAllByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetAllByClub(long ClubID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var taskTargetClub = SchoolClubReader.GetClubAsync(ClubID);
            var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(ClubID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ClubID, cmsUser.ID.Value);


            await Task.WhenAll(taskTargetClub, taskIsUserBanned);
            var targetClub = taskTargetClub.Result;
            var IsUserBanned = taskIsUserBanned.Result;

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
            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;


            IEnumerable<Post_R_PublicDTO> outSet = null;
            if (shouldSanitize)
            {
                outSet = posts
                    .AsParallel()
                    .Select(x =>
                    {
                        x.Content = x.Content.SanitizeHtml();
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }
            else
            {
                outSet = posts
                    .AsParallel()
                    .Select(x =>
                    {
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }


            //check for member status
            var isUserMember = await taskIsUserMember;
            if (!isUserMember)
            {
                outSet = outSet.Where(x => x.IsPublic);
            }

            return Ok(outSet);
        }


        [HttpPost()]
        [Route("GetPageByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageByClub(long ClubID, long? StartID = null, int? PageNum = null, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var taskTargetClub = SchoolClubReader.GetClubAsync(ClubID);
            var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(ClubID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ClubID, cmsUser.ID.Value);

            await Task.WhenAll(taskTargetClub, taskIsUserBanned);
            var targetClub = await taskTargetClub;
            var isUserBanned = await taskIsUserBanned;


            //verify same school
            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            //ensure not banned
            if (isUserBanned)
            {
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            }


            var posts = PostReader.GetPostsByClubPage(ClubID, StartID, PageNum, PageSize);

            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;

            IEnumerable<Post_R_PublicDTO> outSet = null;
            if (shouldSanitize)
            {
                outSet = posts
                    .AsParallel()
                    .Select(x =>
                    {
                        x.Content = x.Content.SanitizeHtml();
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }
            else
            {
                outSet = posts
                    .AsParallel()
                    .Select(x =>
                    {
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }


            //check for member status
            var isUserMember = await taskIsUserMember;
            if (!isUserMember)
            {
                outSet = outSet.Where(x => x.IsPublic);
            }


            //get the highest ent ID from data set
            //this is the pagination StartID
            //used to lock paging to the entities available at initial invocation
            var maxId = outSet.Max(x => x.ID);
            var procSet = new
            {
                StartID = maxId,
                Data = outSet
            };

            return Ok(procSet);
        }


    }
}
