using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Users.Interfaces;

namespace UHub.CoreLib.Users.Management
{
    public static partial class UserReader
    {

        public static IUser_Internal GetAnonymousUser()
        {

            return new User()
            {
                UserID = null,
                Username = "Anon"

            };


        }

    }
}
