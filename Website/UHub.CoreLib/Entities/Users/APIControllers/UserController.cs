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
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/users")]
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
        public async Task<IHttpActionResult> GetMe()
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            var dtoUser = cmsUser.ToDto<User_R_PrivateDTO>();

            return Ok();

        }


        [Route("GetByUname")]
        [HttpPost]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByUname(string username)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var domain = cmsUser.Email.GetEmailDomain();


            //search for user
            var targetUser = await UserReader.GetUserAsync(username, domain);


            //ensure user is found
            if (targetUser == null)
            {
                return NotFound();
            }
            //ensure user is tied to a school
            if (targetUser.SchoolID == null)
            {
                return NotFound();
            }

            //Return full detail if user requests self
            if (targetUser.ID == cmsUser.ID)
            {
                return Ok(cmsUser.ToDto<User_R_PrivateDTO>());
            }
            
            //only allow users to see users from same school
            if(targetUser.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }


            //return partial detail
            return Ok(cmsUser.ToDto<User_R_PublicDTO>());

        }


        [Route("GetByID")]
        [HttpPost]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByID(long UserID )
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            //search for user
            var targetUser = await UserReader.GetUserAsync(UserID);

            //ensure user is found
            if (targetUser == null)
            {
                return NotFound();
            }
            //ensure user is tied to a school
            if (targetUser.SchoolID == null)
            {
                return NotFound();
            }

            //Return full detail if user requests self
            if (targetUser.ID == cmsUser.ID)
            {
                return Ok(cmsUser.ToDto<User_R_PrivateDTO>());
            }

            //only allow users to see users from same school
            if (targetUser.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            //return partial detail
            return Ok(cmsUser.ToDto<User_R_PublicDTO>());

        }

    }
}
