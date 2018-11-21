using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Comments
{
    public enum CommentResultCode
    {
        Success = 0,
        UnknownError = 1,


        ContentInvalid = 1010,
        ContentEmpty = 1011,

    }
}
