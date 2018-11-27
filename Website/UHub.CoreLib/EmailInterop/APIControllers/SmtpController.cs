using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.EmailInterop.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/smtp")]
    public sealed class SmtpController : APIController
    {

        /*/
        [Route("SendTestMail")]
        [HttpGet()]
        public async Task<IHttpActionResult> SendTestMail()
        {
            var msg = new SmtpMessage_ConfirmAcct($"Confirm U-HUB Account (TEST2)", "U-HUB", "aual1780@colorado.edu")
            {
                ConfirmationURL = "https://u-hub.life"
            };

            var start = FailoverDateTimeOffset.UtcNow;
            var val0 = SmtpManager.TrySendMessageAsync(msg);
            var val1 = SmtpManager.TrySendMessageAsync(msg);
            var val2 = SmtpManager.TrySendMessageAsync(msg);
            var val3 = SmtpManager.TrySendMessageAsync(msg);
            var val4 = SmtpManager.TrySendMessageAsync(msg);
            var val5 = SmtpManager.TrySendMessageAsync(msg);

            await Task.WhenAll(val0, val1, val2, val3, val4, val5);
            var end = FailoverDateTimeOffset.UtcNow;


            return Ok($"{(end - start).TotalMilliseconds}ms");

        }
        //*/
    }
}
