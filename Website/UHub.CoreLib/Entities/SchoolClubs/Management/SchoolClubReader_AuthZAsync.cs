using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.Entities.SchoolClubs.Management
{
    public static partial class SchoolClubReader
    {
        

        public static async Task<bool> ValidateMembershipAsync(long ClubID, long UserID)
        {
            return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[SchoolClub_ValidateMembership]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }

        public static async Task<bool> IsUserBannedAsync(long ClubID, long UserID)
        {
            return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[SchoolClub_IsUserBanned]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }

    }
}
