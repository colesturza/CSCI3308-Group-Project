using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using res = System.Web.Http.Results;
using Newtonsoft;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using System.Net;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Users.DTOs;

namespace UHub.CoreLib.Security.Authentication.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/auth")]
    public sealed class AuthenticationController : APIController
    {

        private protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }

            return true;
        }


        [Route("GetToken")]
        [HttpPost()]
        public async Task<IHttpActionResult> GetToken([FromBody] User_CredentialDTO user, bool persistent = false)
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
            string email = user.Email;
            string password = user.Password;


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            status = "Login Failed";
            statCode = HttpStatusCode.BadRequest;

            string token = null;
            try
            {
                //if token generation fails, then create an error message to send to client
                //if detailed errors are enabled, then give brief description
                //if not, then present the same error for all failure types
                var authResultSet = await CoreFactory.Singleton.Auth.TryGetClientAuthTokenAsync(
                    email,
                    password,
                    persistent,
                    GeneralFailHandler: (code) =>
                    {
                        statCode = HttpStatusCode.InternalServerError;
                        if (enableFailCode)
                        {
                            status = code.ToString();
                        }
                    });

                var ResultCode = authResultSet.ResultCode;
                token = authResultSet.AuthToken;

                if (enableDetail)
                {
                    switch (ResultCode)
                    {
                        case AuthResultCode.EmailEmpty: { status = "Email Empty"; break; }
                        case AuthResultCode.EmailInvalid: { status = "Email Invalid"; break; }
                        case AuthResultCode.PswdEmpty: { status = "Password Empty"; break; }
                        case AuthResultCode.UserInvalid: { status = "Account Invalid"; break; }
                        case AuthResultCode.UserLocked: { status = "Account Locked"; break; }
                        case AuthResultCode.PendingApproval: { status = "Account Pending Approval"; break; }
                        case AuthResultCode.PendingConfirmation: { status = "Account Pending Confirmation"; break; }
                        case AuthResultCode.UserDisabled: { status = "Account Disabled"; break; }
                        case AuthResultCode.PswdExpired: { status = "Password Expired"; break; }
                        case AuthResultCode.CredentialsInvalid: { status = "Credentials Invalid"; break; }
                        case AuthResultCode.Success: { status = "Unknown Error"; break; }
                    }
                }
                //this looks strange, but is relevant for a very specific edge case
                //if the auth worker emits a "Success" code without a populated token
                //then this will properly alert the user that some unknown internal error has occured
                if (ResultCode == AuthResultCode.Success)
                {
                    statCode = HttpStatusCode.InternalServerError;
                }


            }
            catch (Exception ex)
            {
                var errCode = "4717C1CF-7C2E-4596-9917-119FF7248B10";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }



            if (token.IsNotEmpty())
            {
                return Ok(token);
            }
            else
            {
                return Content(statCode, status);
            }
        }


        [Route("ExtendToken")]
        [HttpPost()]
        [ApiAuthControl]
        public async Task<IHttpActionResult> ExtendToken()
        {
            string result = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out result, out statCode))
            {
                return Content(statCode, result);
            }

            var token = GetRawAuthToken();


            //in case of any error, fail silent and return the original token
            try
            {
                var resultSet = await CoreFactory.Singleton.Auth.TrySlideAuthTokenExpirationAsync(token);

                string newToken = resultSet.Token;
                var status = resultSet.TokenStatus;
                if (status == TokenValidationStatus.Success)
                {
                    return Ok(newToken);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, newToken);
                }
            }
            catch (Exception ex)
            {
                var errCode = "7D136E21-6F6C-436B-89E3-9F57E6C0861D";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);


                //return original token
                return BadRequest(token);
            }



        }



    }
}
