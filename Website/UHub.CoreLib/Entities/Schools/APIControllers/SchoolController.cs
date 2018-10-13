﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Entities.Schools.Interfaces;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Entities.Users.Interfaces;


namespace UHub.CoreLib.Entities.Schools.APIControllers
{



    [RoutePrefix(Common.API_ROUTE_PREFIX + "/schools")]
    public class SchoolController : APIController
    {




        [HttpGet()]
        [Route("GetSchool")]
        public IHttpActionResult getSchool()
        {
          

            return Ok("test");
        }


    }
}
