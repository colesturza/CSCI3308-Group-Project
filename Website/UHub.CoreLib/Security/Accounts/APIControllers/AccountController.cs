﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.Entities.Schools.DataInterop;
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
        public async Task<IHttpActionResult> CreateUser([FromBody] User_C_PublicDTO user)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var context = System.Web.HttpContext.Current;
            var recaptchaResult = await HandleRecaptchaAsync(context);
            if (!recaptchaResult.IsValid)
            {
                return Content(statCode, recaptchaResult.Result);
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
            bool userCanLogin = false;


            var resultCode = await CoreFactory.Singleton.Accounts.TryCreateUserAsync(
                tmpUser,
                true,
                SuccessHandler: (newUser, canLogin) =>
                {
                    status = "User Created";
                    statCode = HttpStatusCode.OK;
                    userCanLogin = canLogin;
                });

            var isCreated = (resultCode == AcctCreateResultCode.Success);

            if (resultCode == AcctCreateResultCode.UnknownError)
            {
                return Content(HttpStatusCode.InternalServerError, resultCode.ToString());
            }



            if (!isCreated && enableDetail)
            {
                status = resultCode.ToString();
            }



            var responseObj = new
            {
                status,
                canLogin = userCanLogin
            };

            return Content(statCode, responseObj);

        }


    }
}
