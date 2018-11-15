﻿using System;
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
        public ActionResult Login([FromBody] User_CredentialDTO creds)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }


            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = CoreFactory.Singleton.Recaptcha.IsCaptchaValid();
                if (!isCaptchaValid)
                {
                    ViewBag.ErrorMsg = "Captcha is not valid";
                    return View();
                }
            }



            var email = creds.Email;
            var pswd = creds.Password;

            var statusMsg = "An unknown authentication error has occured";
            var isValid = CoreFactory.Singleton.Auth.TrySetClientAuthToken(
                email,
                pswd,
                false,
                out var ResultCode,
                GeneralFailHandler: (id) => { });


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



        public ActionResult Confirm()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            if (idObj == null)
            {
                ViewBag.Message = "Unable to complete operation - must provide confirmation key";
                return View();
            }


            var idStr = idObj.ToString();

            if (CoreFactory.Singleton.Accounts.TryConfirmUser(idStr))
            {
                ViewBag.Message = "User account has been successfully confirmed";
            }
            else
            {
                ViewBag.Message = "User confirmation key is not in a valid format";
            }

            return View();
        }


        [MvcAuthControl]
        public ActionResult UpdatePassword()
        {
            return View();
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }


        [System.Web.Mvc.HttpGet]
        public ActionResult Recover()
        {
            return View();
        }


        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Recover(string txt_Email)
        {
            if (!txt_Email.IsValidEmail())
            {
                ViewBag.Message = "Email is not valid";
                return View();
            }


            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = CoreFactory.Singleton.Recaptcha.IsCaptchaValid();
                if (!isCaptchaValid)
                {
                    ViewBag.Message = "Captcha is not valid";
                    return View();
                }
            }

            var data = await CoreFactory.Singleton.Accounts
                .TryCreateUserRecoveryContextAsync(
                    UserEmail: txt_Email,
                    IsOptional: true,
                    GeneralFailHandler: null);


            if (data.ResultCode != AccountResultCode.Success)
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


            var recoveryMessage = new UHub.CoreLib.SmtpInterop.SmtpMessage_ForgotPswd("UHub Account Recovery", "UHub", txt_Email)
            {
                RecoveryURL = CoreFactory.Singleton.Properties.AcctPswdRecoveryURL,
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