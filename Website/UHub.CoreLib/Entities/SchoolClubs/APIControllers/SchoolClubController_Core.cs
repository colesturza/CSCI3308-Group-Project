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
using UHub.CoreLib.Entities.Posts.Management;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.SchoolClubs.APIControllers
{

    [RoutePrefix(Common.API_ROUTE_PREFIX + "/posts")]
    public sealed partial class SchoolClubController : APIController
    {
        private const short DEFAULT_PAGE_SIZE = 20;


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
