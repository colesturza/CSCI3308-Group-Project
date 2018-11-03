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
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            if(cmsUser != null && cmsUser.ID != null)
            {
                var url = CoreFactory.Singleton.Properties.DefaultAuthFwdURL;
                return Redirect(url);
            }

            return View();
        }


        public ActionResult Confirm()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            if(idObj == null)
            {
                ViewBag.Message = "Unable to complete operation - must provide confirmation key";
                return View();
            }


            var idStr = idObj.ToString();

            if(CoreFactory.Singleton.Accounts.TryConfirmUser(idStr))
            {
                ViewBag.Message = "User account has been successfully confirmed";
            }
            else
            {
                ViewBag.Message = "User confirmation key is not in a valid format";
            }

            return View();
        }





        [System.Web.Mvc.HttpPost]
        public ActionResult ProcessCredentials([FromBody] ClientCreds creds)
        {

            if (!ModelState.IsValid)
            {
                return View("Login");
            }


            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var captcha = CoreFactory.Singleton.Recaptcha.IsCaptchaValid();
                if (!captcha)
                {
                    return View("Login");
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

                var cookieName = CoreFactory.Singleton.Properties.PostAuthCookieName;
                Request.Cookies[cookieName]?.Expire();
                Response.Cookies[cookieName]?.Expire();

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