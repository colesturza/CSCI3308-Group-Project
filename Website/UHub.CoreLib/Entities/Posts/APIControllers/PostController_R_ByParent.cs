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
        [Route("GetPostCountByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPostCountByParent(long ParentID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var taskGetTargetClub = SchoolClubReader.GetClubAsync(ParentID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            var count = 0L;


            //parent is school
            if (ParentID == cmsUser.SchoolID)
            {
                count = await PostReader.GetPostCountByClubAsync(ParentID, true);
            }
            //parent is club
            else
            {
                var targetClub = await taskGetTargetClub;

                if (targetClub != null)
                {

                }
                var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                if (targetClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var isUserMember = await taskIsUserMember;
                var taskGetCount = PostReader.GetPostCountByClubAsync(ParentID, isUserMember);

                if (await taskIsUserBanned)
                {
                    return Ok(0);
                }


                count = await taskGetCount;
            }


            return Ok(count);
        }


        [HttpPost()]
        [Route("GetPageCountByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageCountByParent(long ParentID, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var taskGetTargetClub = SchoolClubReader.GetClubAsync(ParentID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            var count = 0L;


            //parent is school
            if (ParentID == cmsUser.SchoolID)
            {
                count = await PostReader.GetPostCountByClubAsync(ParentID, true);
            }
            //parent is club
            else
            {
                var targetClub = await taskGetTargetClub;

                if (targetClub != null)
                {

                }
                var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                if (targetClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var isUserMember = await taskIsUserMember;
                var taskGetCount = PostReader.GetPostCountByClubAsync(ParentID, isUserMember);

                if (await taskIsUserBanned)
                {
                    return Ok(0);
                }


                count = await taskGetCount;
            }


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
        [Route("GetAllByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetAllByParent(long ParentID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            IEnumerable<Post> posts = null;

            if (ParentID == cmsUser.SchoolID)
            {
                posts = await PostReader.GetPostsByParentAsync(ParentID);
            }
            else
            {
                var taskTargetClub = SchoolClubReader.GetClubAsync(ParentID);
                var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                await Task.WhenAll(taskTargetClub, taskIsUserBanned);

                var targetClub = taskTargetClub.Result;
                bool IsUserBanned = taskIsUserBanned.Result;
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

                posts = await PostReader.GetPostsByParentAsync(ParentID);


                bool IsUserMember = await taskIsUserMember;
                //check for member status
                if (!IsUserMember)
                {
                    posts= posts.Where(x => x.IsPublic);
                }
            }
            



            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;


            IEnumerable<Post_R_PublicDTO> outSet = null;

            if (shouldSanitize)
            {
                outSet = posts
                    .Select(x =>
                    {
                        x.Content = x.Content.SanitizeHtml();
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }
            else
            {
                outSet = posts
                    .Select(x =>
                    {
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }


            return Ok(outSet);
        }


        [HttpPost()]
        [Route("GetPageByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageByParent(long ParentID, long? StartID = null, int? PageNum = null, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            IEnumerable<Post> posts = null;

            if (ParentID == cmsUser.SchoolID)
            {
                posts = await PostReader.GetPostsByParentPageAsync(ParentID,StartID, PageNum, PageSize);
            }
            else
            {
                var taskTargetClub = SchoolClubReader.GetClubAsync(ParentID);
                var taskIsUserBanned = SchoolClubReader.IsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.ValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                await Task.WhenAll(taskTargetClub, taskIsUserBanned);

                var targetClub = taskTargetClub.Result;
                bool IsUserBanned = taskIsUserBanned.Result;
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

                posts = await PostReader.GetPostsByParentPageAsync(ParentID, StartID, PageNum, PageSize);


                bool IsUserMember = await taskIsUserMember;
                //check for member status
                if (!IsUserMember)
                {
                    posts = posts.Where(x => x.IsPublic);
                }
            }


            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;

            IEnumerable<Post_R_PublicDTO> outSet = null;
            if (shouldSanitize)
            {
                outSet = posts
                    .Select(x =>
                    {
                        x.Content = x.Content.SanitizeHtml();
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }
            else
            {
                outSet = posts
                    .Select(x =>
                    {
                        return x.ToDto<Post_R_PublicDTO>();
                    });
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
