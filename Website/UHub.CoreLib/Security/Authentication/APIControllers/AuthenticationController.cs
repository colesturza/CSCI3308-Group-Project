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


namespace UHub.CoreLib.Security.Authentication.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/auth")]
    public sealed class AuthenticationController : APIController
    {

        internal AuthenticationController() : base() { }


        private protected override bool ValidateSystemState(out string error)
        {
            if (!base.ValidateSystemState(out error))
            {
                return false;
            }

            return true;
        }


        [Route("GetToken")]
        [HttpPost()]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public string GetToken(string email, string password, bool persistent = false)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            string error = "";

            if (!base.ValidateSystemState(out error))
            {
                return error;
            }
            if (!HandleRecaptcha(out error))
            {
                return error;
            }


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var er = "Login Failed";
            error = er;


            string token = null;
            try
            {
                //if token generation fails, then create an error message to send to client
                //if detailed errors are enabled, then give brief description
                //if not, then present the same error for all failure types
                token = CoreFactory.Singleton.Auth.TryGetClientAuthToken(
                    email,
                    password,
                    persistent,
                    ResultHandler: (code) =>
                    {
                        if (enableDetail)
                        {
                            error = er;
                        }
                        else
                        {
                            switch (code)
                            {
                                case AuthResultCode.EmailEmpty: { error = "Email Empty"; break; }
                                case AuthResultCode.EmailInvalid: { error = "Email Invalid"; break; }
                                case AuthResultCode.PswdEmpty: { error = "Password Empty"; break; }
                                case AuthResultCode.UserInvalid: { error = "Account Invalid"; break; }
                                case AuthResultCode.UserLocked: { error = "Account Locked"; break; }
                                case AuthResultCode.PendingApproval: { error = "Account Pending Approval"; break; }
                                case AuthResultCode.PendingConfirmation: { error = "Account Pending Confirmation"; break; }
                                case AuthResultCode.UserDisabled: { error = "Account Disabled"; break; }
                                case AuthResultCode.PswdExpired: { error = "Password Expired"; break; }
                                case AuthResultCode.CredentialsInvalid: { error = "Login Failed"; break; }
                                case AuthResultCode.Success: { error = "Unknown Error"; break; }
                            }
                        }

                    },
                    GeneralFailHandler: (code) => { error = enableDetail ? code.ToString() : er; });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                return error;
            }



            if (token.IsNotEmpty())
            {
                return token;
            }
            else
            {
                return error;
            }
        }


        [Route("ExtendToken")]
        [HttpPost()]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public IHttpActionResult ExtendToken(string token)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            if (!base.ValidateSystemState(out var error))
            {
                return new res.NegotiatedContentResult<string>(HttpStatusCode.Forbidden, error, this);
            }

            //in case of any error, fail silent and return the original token
            string newToken = token;
            try
            {
                newToken = CoreFactory.Singleton.Auth.TrySlideAuthTokenExpiration(token);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
            }


            return Ok(newToken);
        }



    }
}
