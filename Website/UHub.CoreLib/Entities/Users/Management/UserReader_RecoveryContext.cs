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

namespace UHub.CoreLib.Entities.Users.Management
{
    public static partial class UserReader
    {

        public static IUserRecoveryContext GetUserRecoveryContext(long UserID)
        {
            try
            {
                return SqlWorker.ExecBasicQuery<UserRecoveryContext>(
                    _dbConn,
                    "[dbo].[User_GetRecoveryContextByUserID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                throw new Exception();
            }
        }

        public static IUserRecoveryContext GetUserRecoveryContext(string RecoveryID)
        {
            try
            {
                return SqlWorker.ExecBasicQuery<UserRecoveryContext>(
                    _dbConn,
                    "[dbo].[User_GetRecoveryContextByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    })
                    .SingleOrDefault();
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                throw new Exception();
            }
        }

    }
}
