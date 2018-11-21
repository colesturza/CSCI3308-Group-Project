using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    public static partial class UserReader
    {
        /// <summary>
        /// Check if user is able to write post to specified ent parent
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<bool> ValidateClubModeratorAsync(long ClubID, long UserID)
        {

            return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[User_ValidateClubModerator]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@CLubID", SqlDbType.BigInt).Value = ClubID;
                });

        }


    }
}
