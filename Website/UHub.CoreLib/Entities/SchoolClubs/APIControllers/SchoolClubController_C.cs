using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.DTOs;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.SchoolClubs.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.APIControllers
{
    public sealed partial class SchoolClubController
    {

        [HttpPost]
        [Route("Create")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> Create([FromBody] SchoolClub_C_PublicDTO club)
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

            if (club == null)
            {
                return BadRequest();
            }


            var tmpClub = club.ToInternal<SchoolClub>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            try
            {
                tmpClub.SchoolID = cmsUser.SchoolID.Value;
                tmpClub.CreatedBy = cmsUser.ID.Value;


                var ClubResult = await SchoolClubManager.TryCreateClubAsync(tmpClub);
                var ClubID = ClubResult.ClubID;
                var ResultCode = ClubResult.ResultCode;


                if (ResultCode == 0)
                {
                    status = "Club Created";
                    statCode = HttpStatusCode.OK;
                }
                else if (ResultCode == SchoolClubResultCode.UnknownError)
                {
                    status = "Unknown server error";
                    statCode = HttpStatusCode.InternalServerError;
                }
                else
                {
                    status = "Invalid Field - " + ResultCode.ToString();
                    statCode = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                var errCode = "d4bcfc43-5247-45a3-b448-5baeea96058e";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);


                statCode = HttpStatusCode.InternalServerError;
            }


            return Content(statCode, status);
        }

    }
}
