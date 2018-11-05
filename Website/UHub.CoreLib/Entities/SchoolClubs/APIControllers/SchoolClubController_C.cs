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
using UHub.CoreLib.Entities.Posts.Management;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.DTOs;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.SchoolClubs.APIControllers
{
    public sealed partial class SchoolClubController
    {

        [HttpPost]
        [Route("Create")]
        [ApiAuthControl]
        public IHttpActionResult Create([FromBody] SchoolClub_C_PublicDTO club)
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

            if(club == null)
            {
                return BadRequest();
            }


            var tmpClub = club.ToInternal<SchoolClub>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();

            try
            {
                tmpClub.SchoolID = cmsUser.SchoolID.Value;
                tmpClub.CreatedBy = cmsUser.ID.Value;

                var clubID = SchoolClubWriter.TryCreateClub(tmpClub);

                if (clubID == null)
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                var errCode = "d4bcfc43-5247-45a3-b448-5baeea96058e";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return Content(HttpStatusCode.InternalServerError, status);
            }


            return Ok();

        }

    }
}
