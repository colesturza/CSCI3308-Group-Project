using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Users.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/user")]
    public sealed class UserController : APIController
    {
        private protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }
            return true;
        }


        [Route("GetMe")]
        [HttpPost]
        [ApiAuthControl]
        public IHttpActionResult GetMe()
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            return Ok(cmsUser.ToDto<User_R_PrivateDTO>());

        }


        [Route("GetByUname")]
        [HttpPost]
        [ApiAuthControl]
        public IHttpActionResult GetByUname(string username)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var domain = cmsUser.Email.GetEmailDomain();


            var targetUser = UserReader.GetUser(username, domain);

            if (targetUser == null)
            {
                return NotFound();
            }
            if (targetUser.SchoolID == null)
            {
                return NotFound();
            }


            if (targetUser.ID == cmsUser.ID)
            {
                return Ok(cmsUser.ToDto<User_R_PrivateDTO>());
            }
            

            if(targetUser.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            return Ok(cmsUser.ToDto<User_R_PublicDTO>());

        }


        [Route("GetByID")]
        [HttpPost]
        [ApiAuthControl]
        public IHttpActionResult GetByID(long UserID )
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            var targetUser = UserReader.GetUser(UserID);

            if (targetUser == null)
            {
                return NotFound();
            }
            if (targetUser.SchoolID == null)
            {
                return NotFound();
            }

            if (targetUser.ID == cmsUser.ID)
            {
                return Ok(cmsUser.ToDto<User_R_PrivateDTO>());
            }

            if (targetUser.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            return Ok(cmsUser.ToDto<User_R_PublicDTO>());

        }

    }
}
