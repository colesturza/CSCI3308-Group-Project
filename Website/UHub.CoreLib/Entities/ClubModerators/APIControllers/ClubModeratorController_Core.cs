﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;

namespace UHub.CoreLib.Entities.ClubModerators.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/clubModerators")]
    public sealed partial class ClubModeratorController : APIController
    {

        private protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }
            return true;
        }

    }
}
