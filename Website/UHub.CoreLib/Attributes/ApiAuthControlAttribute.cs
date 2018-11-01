﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using UHub.CoreLib.Management;
using UHub.CoreLib.Extensions;
using System.Web;

namespace UHub.CoreLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ApiAuthControlAttribute : AuthorizeAttribute
    {
        public bool RequireAdmin { get; set; }


        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                bool isLoggedIn = false;
                var isValid = actionContext.Request.Headers.TryGetValues(Common.AUTH_HEADER_TOKEN, out var valueSet);


                string authToken = "";
                if (isValid && valueSet != null)
                {
                    authToken = valueSet.FirstOrDefault();
                }


                if (authToken.IsEmpty())
                {
                    //test for cookie auth
                    isLoggedIn = CoreFactory.Singleton.Auth.IsUserLoggedIn(out _, out _);
                }
                else
                {
                    //test for token auth
                    isLoggedIn = CoreFactory.Singleton.Auth.TrySetRequestUser(authToken, out _);
                }

                if(!isLoggedIn)
                {
                    return false;
                }


                var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
                
                return cmsUser.ID != null && cmsUser.IsEnabled && (!RequireAdmin || cmsUser.IsAdmin);
            }
            catch (Exception ex)
            {
                var errCode = "EA1A7A06-36FD-4276-9D7D-095A83C2E513";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);

                return false;
            }
        }

    }
}