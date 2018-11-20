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
        public static (long? ClubID, SchoolClubResultCode ResultCode) TryCreateClub(SchoolClub NewClub)
        {
            Shared.TryCreate_HandleAttrTrim(ref NewClub);

            Shared.TryCreate_AttrConversionHandler(ref NewClub);

            var attrValidateCode = Shared.TryCreate_ValidateClubAttrs(NewClub);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }


            var id = SchoolClubWriter.TryCreateClub(NewClub);
            return (id, SchoolClubResultCode.Success);

        }

    }
#pragma warning restore

}
