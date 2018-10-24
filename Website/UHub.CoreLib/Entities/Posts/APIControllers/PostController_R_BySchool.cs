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
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.Posts.APIControllers
{

    public sealed partial class PostController
    {


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
        [Route("GetPageCountBySchool")]
        [ApiAuthControl]
        public IHttpActionResult GetPageCountBySchool(short PageSize = DEFAULT_PAGE_SIZE)
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
        [Route("GetPageBySchool")]
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









    }
}
