using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Security.Accounts;
using UHub.CoreLib.Security.Authentication;
using UHub.CoreLib.SmtpInterop;

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


        public ActionResult Create()
        {
            return View();
        }



        [System.Web.Mvc.HttpGet]
        public ActionResult Login()
        {
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            if (cmsUser != null && cmsUser.ID != null)
            {
                var url = CoreFactory.Singleton.Properties.DefaultAuthFwdURL;
                return Redirect(url);
            }

            return View();
        }


        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Login([FromBody] User_CredentialDTO creds)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.ErrorMsg = "Captcha is not valid";
                    return View();
                }
            }



            var email = creds.Email;
            var pswd = creds.Password;

            var statusMsg = "An unknown authentication error has occured";
            var ResultCode = CoreFactory.Singleton.Auth.TrySetClientAuthToken(
                email,
                pswd,
                false,
                GeneralFailHandler: (id) => { });

            var isValid = (ResultCode == AuthResultCode.Success);

            switch (ResultCode)
            {
                case AuthResultCode.EmailEmpty:
                    {
                        statusMsg = "Email cannot be Empty";
                        break;
                    }
                case AuthResultCode.EmailInvalid:
                    {
                        statusMsg = "Email address is invalid";
                        break;
                    }
                case AuthResultCode.PswdEmpty:
                    {
                        statusMsg = "Password cannot be empty";
                        break;
                    }
                case AuthResultCode.PswdInvalid:
                    {
                        statusMsg = "Password is invalid";
                        break;
                    }
                case AuthResultCode.PswdExpired:
                    {
                        statusMsg = "Password has expired - please reset and try again";
                        break;
                    }
                case AuthResultCode.UserInvalid:
                    {
                        statusMsg = "User account is invalid";
                        break;
                    }
                case AuthResultCode.UserLocked:
                    {
                        statusMsg = "User account is currently locked - please try again later";
                        break;
                    }
                case AuthResultCode.UserDisabled:
                    {
                        statusMsg = "User account is disabled";
                        break;
                    }
                case AuthResultCode.PendingApproval:
                    {
                        statusMsg = "User account is pending admin approval";
                        break;
                    }
                case AuthResultCode.PendingConfirmation:
                    {
                        statusMsg = "User account is pending email confirmation";
                        break;
                    }
                case AuthResultCode.CredentialsInvalid:
                    {
                        statusMsg = "User credentials are invalid";
                        break;
                    }
                case AuthResultCode.Success:
                    {
                        statusMsg = "Success";
                        break;
                    }
            }



            if (isValid)
            {
                var url = CoreFactory.Singleton.Auth.GetAuthForwardUrl();

                var cookieName = CoreFactory.Singleton.Properties.PostAuthCookieName;
                Request.Cookies[cookieName]?.Expire();
                Response.Cookies[cookieName]?.Expire();

                return Redirect(url);
            }
            else
            {
                ViewBag.ErrorMsg = statusMsg;

                return View();
            }
        }


        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Confirm()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            if (idObj == null)
            {
                ViewBag.Message = "Unable to complete operation - must provide confirmation key";
                return View();
            }


            var idStr = idObj.ToString();

            var confResult = await CoreFactory.Singleton.Accounts.TryConfirmUserAsync(idStr);
            if (confResult == 0)
            {
                ViewBag.Message = "User account has been successfully confirmed";
            }
            else
            {
                ViewBag.Message = "User confirmation failed - " + confResult.ToString();
            }

            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult UpdatePassword()
        {
            ViewBag.CanForward = false;
            return View();
        }


        [System.Web.Mvc.HttpPost]
        [MvcAuthControl]
        public async Task<ActionResult> UpdatePassword(string txt_CurrentPswd, string txt_NewPswd, string txt_ConfirmPswd, bool chk_DeviceLogout = false)
        {
            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.ErrorMsg = "Captcha is not valid";
                    return View();
                }
            }


            if (txt_NewPswd != txt_ConfirmPswd)
            {
                ViewBag.Message = "Passwords must match";
                return View();
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


            var result = await CoreFactory.Singleton.Accounts.TryUpdatePasswordAsync(
                cmsUser.ID.Value,
                txt_CurrentPswd,
                txt_NewPswd,
                chk_DeviceLogout,
                context);


            ViewBag.Message = result.ToString();

            ViewBag.CanForward = (result == 0);

            return View();
        }



        [System.Web.Mvc.HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }



        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> ForgotPassword(string txt_Email)
        {
            if (!txt_Email.IsValidEmail())
            {
                ViewBag.Message = "Email is not valid";
                return View();
            }


            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.ErrorMsg = "Captcha is not valid";
                    return View();
                }
            }


            var data = await CoreFactory.Singleton.Accounts
                .TryCreateUserRecoveryContextAsync(
                    UserEmail: txt_Email,
                    IsOptional: true,
                    GeneralFailHandler: null);


            if (data.ResultCode != AcctRecoveryResultCode.Success)
            {
                //*/
                ViewBag.Message = data.ResultCode.ToString();
                /*/
                ViewBag.Message = "Recovery email sent, please check your inbox";
                //*/

                //"security" measure
                //make it more difficult to discover valid email addresses
                Random rnd = new Random();
                var fluff = rnd.Next(1900, 2400);
                await Task.Delay(fluff);
                return View();
            }


            var path = data.RecoveryContext.RecoveryURL;

            var recoveryMessage = new UHub.CoreLib.SmtpInterop.SmtpMessage_ForgotPswd("UHub Account Recovery", "UHub", txt_Email)
            {
                RecoveryURL = path,
                RecoveryKey = data.RecoveryKey
            };


            var mailResult = await CoreFactory.Singleton.Mail.TrySendMessageAsync(recoveryMessage);

            if (mailResult == SmtpResultCode.Success)
            {
                ViewBag.Message = "Recovery email sent, please check your inbox";
                return View();
            }
            else
            {
                ViewBag.Message = mailResult.ToString();
                return View();
            }
        }



        [System.Web.Mvc.HttpGet]
        public ActionResult Recover()
        {
            ViewBag.CanForward = false;
            return View();
        }


        public async Task<ActionResult> Recover(string txt_RecoveryKey, string txt_NewPswd, string txt_ConfirmPswd)
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var recoveryID = idObj?.ToString() ?? "";

            if (recoveryID.IsEmpty() || recoveryID.Length > 200)
            {
                ViewBag.Message = "Invalid Recovery Key";
                return View();
            }

            if (txt_RecoveryKey.Length > 200)
            {
                ViewBag.Message = "Invalid Recovery Key";
                return View();
            }

            if (txt_NewPswd != txt_ConfirmPswd)
            {
                ViewBag.Message = "Passwords must match";
                return View();
            }


            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.ErrorMsg = "Captcha is not valid";
                    return View();
                }
            }



            var result = await CoreFactory.Singleton.Accounts.TryRecoverPasswordAsync(
                recoveryID,
                txt_RecoveryKey,
                txt_NewPswd,
                true,
                context);


            ViewBag.Message = result.ToString();

            ViewBag.CanForward = (result == 0);



            return View();
        }


        [MvcAuthControl]
        public ActionResult Find()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "0";
            var valid = int.TryParse(idStr, out var id);

            return View();
        }
    }
}