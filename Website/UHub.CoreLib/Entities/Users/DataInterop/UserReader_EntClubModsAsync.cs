using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

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
        public static async Task<bool> TryValidateClubModeratorAsync(long ClubID, long UserID)
        {
            try
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
            catch (Exception ex)
            {
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync("99D05263-9224-41D4-B097-12EE87647303", ex);
                return false;
            }

        }


    }
}
