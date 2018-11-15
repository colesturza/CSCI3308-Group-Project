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
        [Route("GetAll")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAll()
        {

            var schoolSet = await SchoolReader.GetAllSchoolsAsync();

            
            return Ok(schoolSet.Select(x => x.ToDto<School_R_PublicDTO>()));

        }


        [HttpGet()]
        [Route("GetByID")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetByID(long SchoolID)
        {

            var school = await SchoolReader.GetSchoolAsync(SchoolID);

            if(school == null)
            {
                return NotFound();
            }


            return Ok(school.ToDto<School_R_PublicDTO>());

        }


        [HttpGet()]
        [Route("IsEmailValid")]
        [ApiCacheControl(1 * 3600)]
        public async Task<IHttpActionResult> IsEmailValid(string email)
        {
            if (await SchoolReader.IsEmailValidAsync(email))
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
        public async Task<IHttpActionResult> IsDomainValid(string domain)
        {

            if (await SchoolReader.IsDomainValidAsync(domain))
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
