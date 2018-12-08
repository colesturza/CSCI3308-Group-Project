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
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.Entities.SchoolMajors.DTOs;

namespace UHub.CoreLib.Entities.SchoolMajors.APIControllers
{
    [RoutePrefix(Common.API_ROUTE_PREFIX + "/schoolmajors")]
    public class SchoolMajorController : APIController
    {

        [HttpGet]
        [Route("GetAllBySchool")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAllBySchool(long SchoolID)
        {

            var majorSet = await SchoolMajorReader.TryGetMajorsBySchoolAsync(SchoolID);
            if (majorSet == null)
            {
                return InternalServerError();
            }


            return Ok(majorSet.Select(x => x.ToDto<SchoolMajor_R_PublicDTO>()));

        }


        [HttpGet]
        [Route("GetAllByEmail")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAllByEmail(string Email)
        {
            if (!Email.IsValidEmail())
            {
                return BadRequest();
            }

            var majorSet = await SchoolMajorReader.TryGetMajorsByEmailAsync(Email);
            if (majorSet == null)
            {
                return InternalServerError();
            }


            return Ok(majorSet.Select(x => x.ToDto<SchoolMajor_R_PublicDTO>()));

        }


        [HttpGet]
        [Route("GetAllByDomain")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAllByDomain(string Domain)
        {
            if (!Domain.IsValidEmailDomain())
            {
                return BadRequest();
            }


            var majorSet = await SchoolMajorReader.TryGetMajorsByDomainAsync(Domain);
            if (majorSet == null)
            {
                return InternalServerError();
            }


            return Ok(majorSet.Select(x => x.ToDto<SchoolMajor_R_PublicDTO>()));

        }

    }
}
