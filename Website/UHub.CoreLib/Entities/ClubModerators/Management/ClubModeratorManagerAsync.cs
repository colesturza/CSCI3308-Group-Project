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
        public static async Task<(long? ClubModID, ClubModeratorResultCode ResultCode)> TryCreateClubModeratorAsync(ClubModerator NewModerator, long ParentID)
        {
            var id = await ClubModeratorWriter.TryCreateClubModeratorAsync(NewModerator, ParentID);

            return (id, ClubModeratorResultCode.Success);
        }

    }
#pragma warning restore

}
