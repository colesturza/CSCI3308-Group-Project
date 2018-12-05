﻿using System;
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
        public async Task<IHttpActionResult> Create([FromBody] SchoolClub_C_PublicDTO ClubDTO)
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

            if (ClubDTO == null)
            {
                return BadRequest();
            }


            var clubInternal = ClubDTO.ToInternal<SchoolClub>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            try
            {
                clubInternal.SchoolID = cmsUser.SchoolID.Value;
                clubInternal.CreatedBy = cmsUser.ID.Value;


                var ClubResult = await SchoolClubManager.TryCreateClubAsync(clubInternal);
                var ClubID = ClubResult.ClubID;
                var ResultCode = ClubResult.ResultCode;


                if (ResultCode == 0)
                {
                    status = ClubID.ToString();
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
                var errCode = "C3546EA7-AF05-43C0-A9F0-3C6E724EFF20";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);


                statCode = HttpStatusCode.InternalServerError;
            }


            return Content(statCode, status);
        }

    }
}
