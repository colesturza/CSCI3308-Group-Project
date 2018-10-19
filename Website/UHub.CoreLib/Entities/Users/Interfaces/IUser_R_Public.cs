using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    /// <summary>
    /// CMS User publically exposable interface
    /// </summary>
    public interface IUser_R_Public
    {
        
        string Username { get; }
        string Major { get; set; }
        string Year { get; set; }
        string GradDate { get; set; }
        string Company { get; set; }
        string JobTitle { get; set; }


    }
}
