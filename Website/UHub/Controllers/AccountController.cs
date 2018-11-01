using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using UHub.Models.Account;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Security.Authentication;

namespace UHub.Controllers
{
    public class AccountController : Controller
    {
        [MvcAuthControl]
        public ActionResult Index()
        {
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            var userName = cmsUser.Username;

            ViewBag.Username = userName.HtmlEncode();

            return View();
        }


        public ActionResult Login()
        {

            return View();
        }


        public ActionResult ProcessCredentials([FromBody] ClientCreds creds)
        {
            if (!ModelState.IsValid)
            {
                return Login();
            }


            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var captcha = CoreFactory.Singleton.Recaptcha.IsCaptchaValid();
                if (!captcha)
                {
                    return Login();
                }
            }

            var email = creds.Email;
            var pswd = creds.Password;

            var status = "An unknown authentication error has occured";
            CoreFactory.Singleton.Auth.TrySetClientAuthToken(email, pswd, false,
                ResultHandler: (ResultCode) =>
                {
                    switch (ResultCode)
                    {
                        case AuthResultCode.EmailEmpty:
                            {
                                status = "Email cannot be Empty";
                                break;
                            }
                        case AuthResultCode.EmailInvalid:
                            {
                                status = "Email address is invalid";
                                break;
                            }
                        case AuthResultCode.PswdEmpty:
                            {
                                status = "Password cannot be empty";
                                break;
                            }
                        case AuthResultCode.PswdInvalid:
                            {
                                status = "Password is invalid";
                                break;
                            }
                        case AuthResultCode.PswdExpired:
                            {
                                status = "Password has expired - please reset and try again";
                                break;
                            }
                        case AuthResultCode.UserInvalid:
                            {
                                status = "User account is invalid";
                                break;
                            }
                        case AuthResultCode.UserLocked:
                            {
                                status = "User account is currently locked - please try again later";
                                break;
                            }
                        case AuthResultCode.UserDisabled:
                            {
                                status = "User account is disabled";
                                break;
                            }
                        case AuthResultCode.PendingApproval:
                            {
                                status = "User account is pending admin approval";
                                break;
                            }
                        case AuthResultCode.PendingConfirmation:
                            {
                                status = "User account is pending email confirmation";
                                break;
                            }
                        case AuthResultCode.CredentialsInvalid:
                            {
                                status = "User credentials are invalid";
                                break;
                            }
                        case AuthResultCode.Success:
                            {
                                status = "Success";
                                break;
                            }
                    }

                },
                GeneralFailHandler: (id) => { });



            if (status == "Success")
            {
                var url = CoreFactory.Singleton.Auth.GetAuthForwardUrl();
                return Redirect(url);
            }
            else
            {
                ViewBag.ErrorMsg = status;

                return View("Login");
            }
        }
    }
}