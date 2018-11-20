using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;

namespace UHub.CoreLib.Entities.SchoolClubs.Management
{
#pragma warning disable 612,618

    public static partial class SchoolClubManager
    {
        public static async Task<(long? ClubID, SchoolClubResultCode ResultCode)> TryCreateClubAsync(SchoolClub NewClub)
        {
            Shared.TryCreate_HandleAttrTrim(ref NewClub);

            Shared.TryCreate_AttrConversionHandler(ref NewClub);

            var attrValidateCode = Shared.TryCreate_ValidateClubAttrs(NewClub);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }


            var id = await SchoolClubWriter.TryCreateClubAsync(NewClub);
            if (id == null)
            {
                return (id, SchoolClubResultCode.UnknownError);
            }
            return (id, SchoolClubResultCode.Success);

        }

    }
#pragma warning restore

}
