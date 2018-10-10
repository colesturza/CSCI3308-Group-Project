﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Authentication
{
    internal enum PasswordValidationStatus
    {
        Success,
        NotFound,
        InvalidUser,
        HashMismatch,
        PswdExpired,
        UnknownError
    }
}
