﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.ClubModerators.DTOs;
using UHub.CoreLib.Entities.ClubModerators.Management;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.ClubModerators.APIControllers
{
    public sealed partial class ClubModeratorController
    {
        [HttpPost()]
        [Route("Create")]
        [ApiAuthControl]
        public IHttpActionResult Create([FromBody] ClubModerator_C_PublicDTO clubModerator, long clubID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }
            if (!HandleRecaptcha(out status))
            {
                return Content(statCode, status);
            }

            var tmpClubModerator = clubModerator.ToInternal<ClubModerator>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            bool isCurrentUserOwner = false;
            bool isCurrentUserBanned = true;
            bool isUserBanned = true;

            TaskList tasks = new TaskList();
            tasks.Add(() => { isCurrentUserOwner = UserReader.ValidateClubModerator(clubID, (long)cmsUser.ID); });
            tasks.Add(() => { isCurrentUserBanned = SchoolClubReader.IsUserBanned(clubID, (long)cmsUser.ID); });
            tasks.Add(() => { isUserBanned = SchoolClubReader.IsUserBanned(clubID, tmpClubModerator.UserID); });
            tasks.ExecuteAll();

            if (isCurrentUserBanned || !isCurrentUserOwner)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            if (isUserBanned)
            {
                status = "User is banned";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }


            status = "Failed to create post.";
            statCode = HttpStatusCode.BadRequest;

            try
            {

                long? clubModID = ClubModeratorWriter.TryCreateClubModerator(tmpClubModerator, clubID);

                if (clubModID != null)
                {
                    status = "Club moderator created.";
                    statCode = HttpStatusCode.OK;
                }

            }
            catch (Exception ex)
            {
                var errCode = "185AB13F-2C5C-435B-8B87-AA48F1AB3C73";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }


            return Content(statCode, status);

        }
    }
}