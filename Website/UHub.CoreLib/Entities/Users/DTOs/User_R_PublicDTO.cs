using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Entities.Users.DTOs
{
    [DtoClass(typeof(User))]
    public sealed partial class User_R_PublicDTO : DtoEntityBase, IUser_R_Public
    {
        public string Username { get; set; }
        public string Major { get; set; }
        public string Year { get; set; }
        public string GradDate { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
    }
}
