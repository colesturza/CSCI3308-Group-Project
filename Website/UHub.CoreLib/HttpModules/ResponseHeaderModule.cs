using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UHub.CoreLib.HttpModules
{
    /// <summary>
    /// Configure page response headers
    /// </summary>
    public sealed class ResponseHeaderModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }


        public void Init(HttpApplication context)
        {
            //lock (lockObject)
            //{
            //context.EndRequest += Context_EndRequest;
            context.PostReleaseRequestState += Context_PostReleaseRequestState;
            //}
        }



        private void Context_PostReleaseRequestState(object sender, EventArgs e)
        {
            var Response = ((HttpApplication)sender).Context.Response;

            //ensure headers have not already been written
            //this prevents errors with file downloads
            if (Response.HeadersWritten)
            {
                return;
            }

            //Response.ClearHeaders();

            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-Powered-By");

            void doHeaderWork(string name, string val)
            {
                Response.Headers.Remove(name);
                Response.Headers.Add(name, val);
            };

            //set response headers to limit site XSS vectors and frame access
            doHeaderWork("Vary", "Accept-Encoding");
            doHeaderWork("X-UA-Compatible", "IE=edge");
            doHeaderWork("Access-Control-Allow-Methods", "GET");
            doHeaderWork("X-XSS-Protection", "1; mode=block");
            doHeaderWork("X-Content-Type-Options", "nosniff");
            doHeaderWork("X-Frame-Options", "sameorigin");

        }

        #endregion

    }
}
