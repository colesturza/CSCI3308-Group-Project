using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Entities.SchoolMajors.Management;
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Security.Accounts.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/account")]
    public sealed class AccountController : APIController
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
        [Route("CreateUser")]
        public IHttpActionResult CreateUser([FromBody] User_C_PublicDTO user)
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

            if (user == null)
            {
                return BadRequest();
            }


            var tmpUser = user.ToInternal<User>();


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            status = "Account Creation Failed";
            statCode = HttpStatusCode.BadRequest;

            try
            {
                var isCreated = CoreFactory.Singleton.Accounts.TryCreateUser(
                    tmpUser,
                    true,
                    out var resultCode,
                    (code) =>
                    {
                        statCode = HttpStatusCode.InternalServerError;
                        if (enableFailCode)
                        {
                            status = code.ToString();
                        }
                    },
                    (newUser, canLogin) =>
                    {
                        status = "User Created";
                        statCode = HttpStatusCode.OK;
                    });


                if (!isCreated && enableDetail)
                {
                    switch (resultCode)
                    {
                        case AccountResultCode.EmailEmpty: { status = "Email Empty"; break; }
                        case AccountResultCode.EmailInvalid: { status = "Email Invalid"; break; }
                        case AccountResultCode.EmailDuplicate: { status = "Email Duplicate"; break; }
                        case AccountResultCode.EmailDomainInvalid: { status = "Email Domain Not Supported"; break; }
                        case AccountResultCode.UsernameDuplicate: { status = "Username Duplicate"; break; }
                        case AccountResultCode.UserInvalid: { status = "User is not valid"; break; }
                        case AccountResultCode.MajorInvalid: { status = "Major Invalid"; break; }
                        case AccountResultCode.PswdEmpty: { status = "Password Empty"; break; }
                        case AccountResultCode.PswdInvalid: { status = "Password Invalid"; break; }
                        case AccountResultCode.UnknownError: { status = "An unknown error has occured"; break; }
                    }
                }
            }
            catch (Exception ex)
            {
                var errCode = "100d1257-b74c-461d-a389-b90298895e5d";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }


            return Content(statCode, status);

        }


    }
}
