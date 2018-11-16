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
using UHub.CoreLib.Entities.Posts.Management;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.Posts.APIControllers
{

    public sealed partial class PostController
    {


        [HttpPost()]
        [Route("GetPostCountBySchool")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPostCountBySchool()
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var userID = cmsUser.ID.Value;
            var schoolID = cmsUser.SchoolID.Value;


            var taskGetUserMemberships = UserReader.GetValidClubMembershipsAsync(userID);
            var taskGetCountSet = PostReader.GetPostClusteredCountsAsync(schoolID);

            var membershipHash = (await taskGetUserMemberships).ToHashSet();
            await taskGetCountSet;


            var count = 0L;

            foreach(var counter in taskGetCountSet.Result)
            {
                count += counter.PublicPostCount;
                if(counter.SchoolClubID != null && membershipHash.Contains(counter.SchoolClubID.Value))
                {
                    count += counter.PrivatePostCount;
                }
            }


            return Ok(count);

        }

        [HttpPost()]
        [Route("GetPageCountBySchool")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageCountBySchool(short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var userID = cmsUser.ID.Value;
            var schoolID = cmsUser.SchoolID.Value;


            var taskGetUserMemberships = UserReader.GetValidClubMembershipsAsync(userID);
            var taskGetCountSet = PostReader.GetPostClusteredCountsAsync(schoolID);

            var membershipHash = (await taskGetUserMemberships).ToHashSet();
            await taskGetCountSet;


            var count = 0L;

            foreach (var counter in taskGetCountSet.Result)
            {
                count += counter.PublicPostCount;
                if (counter.SchoolClubID != null && membershipHash.Contains(counter.SchoolClubID.Value))
                {
                    count += counter.PrivatePostCount;
                }
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


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var schoolID = cmsUser.SchoolID.Value;


            var posts = PostReader.GetPostsBySchool(schoolID);


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


        //[HttpPost()]
        //[Route("GetAllBySchool")]
        //[ApiAuthControl]
        //public async Task<IHttpActionResult> GetAllBySchool()
        //{
        //    string status = "";
        //    HttpStatusCode statCode = HttpStatusCode.BadRequest;
        //    if (!this.ValidateSystemState(out status, out statCode))
        //    {
        //        return Content(statCode, status);
        //    }


        //    var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
        //    var schoolID = cmsUser.SchoolID.Value;


        //    var posts = await PostReader.GetPostsBySchoolAsync(schoolID);


        //    var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
        //    var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;


        //    IEnumerable<Post_R_PublicDTO> outSet = null;
        //    if (shouldSanitize)
        //    {
        //        outSet = posts
        //            .Select(x =>
        //            {
        //                x.Content = x.Content.SanitizeHtml();
        //                return x.ToDto<Post_R_PublicDTO>();
        //            });
        //    }
        //    else
        //    {
        //        outSet = posts
        //            .Select(x =>
        //            {
        //                return x.ToDto<Post_R_PublicDTO>();
        //            });
        //    }


        //    return Ok(outSet);

        //}

        [HttpPost()]
        [Route("GetPageBySchool")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageBySchool(long? StartID = null, int? PageNum = null, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var schoolID = cmsUser.SchoolID.Value;


            var posts = await PostReader.GetPostsBySchoolPageAsync(schoolID, StartID, PageNum, PageSize);

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
