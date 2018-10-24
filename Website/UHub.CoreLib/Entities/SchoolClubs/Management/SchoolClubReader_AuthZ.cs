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
        

        public static bool ValidateMembership(long ClubID, long UserID)
        {
            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                "[dbo].[SchoolClub_ValidateMembership]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }

        public static bool IsUserBanned(long ClubID, long UserID)
        {
            return SqlWorker.ExecScalar<bool>(
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
