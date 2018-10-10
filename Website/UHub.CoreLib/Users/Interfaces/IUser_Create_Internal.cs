using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Users.Interfaces
{
    public interface IUser_Create_Internal : IUser_Create_Public
    {
        long? SchoolID { get; set; }
        string Version { get; set; }
        bool IsApproved { get; set; }
        bool IsConfirmed { get; set; }
        bool IsFinished { get; set; }
    }
}
