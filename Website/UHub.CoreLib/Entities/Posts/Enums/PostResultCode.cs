using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Posts
{
    public enum PostResultCode : short
    {
        Success = 0,
        UnknownError = 1,

        NameInvalid = 1010,
        NameEmpty = 1011,
        ContentInvalid = 1020,
        ContentEmpty = 1021,
        

    }
}
