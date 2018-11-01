using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.ClubModerators.Interfaces
{
    public interface IClubModerator_C_Public
    {
        bool IsEnabled { get; set; }

        bool IsReadOnly { get; set; }

        long UserID { get; set; }

        bool IsOwner { get; set; }
    }
}
