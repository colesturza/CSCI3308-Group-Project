using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    public static partial class UserReader
    {

        public static async Task<IUserRecoveryContext> GetRecoveryContextAsync(long UserID)
        {
            try
            {
                var temp = await SqlWorker.ExecBasicQueryAsync<UserRecoveryContext>(
                    _dbConn,
                    "[dbo].[User_GetRecoveryContextByUserID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });


                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                throw new Exception();
            }
        }

        public static async Task<IUserRecoveryContext> GetRecoveryContextAsync(string RecoveryID)
        {
            try
            {
                var temp = await SqlWorker.ExecBasicQueryAsync<UserRecoveryContext>(
                    _dbConn,
                    "[dbo].[User_GetRecoveryContextByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    });

                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                throw new Exception();
            }
        }

    }
}
