using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.ClubModerators.DataInterop;

namespace UHub.CoreLib.Entities.ClubModerators.Management
{
#pragma warning disable 612, 618
    public static partial class ClubModeratorManager
    {
        public static (long? ClubModID, ClubModeratorResultCode ResultCode) TryCreateClubModerator(ClubModerator NewModerator, long ParentID)
        {
            var id = ClubModeratorWriter.TryCreateClubModerator(NewModerator, ParentID);

            return (id, ClubModeratorResultCode.Success);
        }

    }
#pragma warning restore

}
