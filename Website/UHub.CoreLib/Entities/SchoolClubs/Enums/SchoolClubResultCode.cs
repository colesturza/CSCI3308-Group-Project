using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.SchoolClubs
{
    public enum SchoolClubResultCode
    {
        Success = 0,
        UnknownError = 1,


        NameInvalid = 1010,
        NameEmpty = 1011,
        DescriptionInvalid = 1020,

    }
}
