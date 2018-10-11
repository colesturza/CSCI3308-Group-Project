using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Users.Interfaces
{
    public interface IUser_Update_Private
    {
        long? UserID { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string Major { get; set; }
        string Year { get; set; }
        string GradDate{ get; set; }
        string Company { get; set; }
        string JobTitle { get; set; }

    }
}
