using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MvcAuthControlAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public bool RequireAdmin { get; set; }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            try
            {
                bool isLoggedIn = false;
                var authToken = filterContext.HttpContext.Request.Headers.Get(Common.AUTH_HEADER_TOKEN);



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

                if (!isLoggedIn)
                {
                    HandleLoginRedirect(ref filterContext);
                    return;
                }


                var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();


                var isValid = cmsUser.ID != null && cmsUser.IsEnabled && (!RequireAdmin || cmsUser.IsAdmin);


                if (!isValid)
                {
                    HandleLoginRedirect(ref filterContext);
                    return;
                }


            }
            catch (Exception ex)
            {
                var errCode = "C39F81BF-E61D-4C55-AA0D-E8950549E74B";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);


                HandleLoginRedirect(ref filterContext);
            }
        }


        private void HandleLoginRedirect(ref AuthorizationContext filterContext)
        {
            var loginAddr = CoreFactory.Singleton.Properties.LoginURL;
            var fwrdCookieName = CoreFactory.Singleton.Properties.PostAuthCookieName;



            var targetAddr = filterContext.HttpContext.Request.Url.AbsoluteUri;
            var cookie = new HttpCookie(fwrdCookieName);
            cookie.Value = targetAddr;
            cookie.Domain = CoreFactory.Singleton.Properties.CookieDomain;
            cookie.Encrypt();
            filterContext.HttpContext.Response.SetCookie(cookie);



            filterContext.Result = new RedirectResult(loginAddr);


        }

    }
}
