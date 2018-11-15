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
        [Route("GetAllBySchool")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetAllBySchool()
        {

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser();
            var schoolID = cmsUser.SchoolID.Value;

            var clubSet = await SchoolClubReader.GetClubsBySchoolAsync(schoolID);


            return Ok(clubSet.Select(x=>x.ToDto<SchoolClub_R_PublicDTO>()));
        }

    }
}
