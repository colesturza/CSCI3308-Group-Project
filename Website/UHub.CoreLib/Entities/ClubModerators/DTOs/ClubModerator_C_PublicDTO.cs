using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.ClubModerators.Interfaces;

namespace UHub.CoreLib.Entities.ClubModerators.DTOs
{
    [DtoClass(typeof(ClubModerator))]
    public sealed partial class ClubModerator_C_PublicDTO : DtoEntityBase, IClubModerator_C_Public
    {
        public long UserID { get; set; }
        public bool IsOwner { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
