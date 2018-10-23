using Newtonsoft.Json;
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
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Entities.Schools.DTOs;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Attributes;

namespace UHub.CoreLib.Entities.Schools.APIControllers
{



    [RoutePrefix(Common.API_ROUTE_PREFIX + "/schools")]
    public class SchoolController : APIController
    {

        [HttpGet()]
        [Route("GetSchools")]
        [ApiCacheControl(12 * 3600)]
        public IHttpActionResult GetAllSchools()
        {

            var schoolSet = SchoolReader.GetAllSchools();

            
            return Ok(schoolSet.Select(x => x.ToDto<School_R_PublicDTO>()));

        }


        [HttpGet()]
        [Route("IsEmailValid")]
        [ApiCacheControl(1 * 3600)]
        public IHttpActionResult IsEmailValid(string email)
        {
            if (SchoolReader.IsEmailValid(email))
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }

        }


        [HttpGet()]
        [Route("IsDomainValid")]
        [ApiCacheControl(1 * 3600)]
        public IHttpActionResult IsDomainValid(string domain)
        {

            if (SchoolReader.IsDomainValid(domain))
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }

        }

    }
}
