using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    public static partial class SchoolClubReader
    {


        public static async Task<bool> TryValidateMembershipAsync(long ClubID, long UserID)
        {
            try
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
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("92A4A7C6-FA84-4E58-BACB-076FF9C806E4", ex);
                return false;
            }
        }


        public static async Task<bool> TryIsUserBannedAsync(long ClubID, long UserID)
        {

            try
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
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("BDC177ED-9A72-4A50-90D4-AF03CB32E9EB", ex);
                return true;
            }
        }

    }
}
