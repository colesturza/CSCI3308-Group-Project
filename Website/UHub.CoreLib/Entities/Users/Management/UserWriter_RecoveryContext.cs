using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.Entities.Users.Management
{
    internal static partial class UserWriter
    {
        /// <summary>
        /// Create an account recovery context for a specified user
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="RecoveryKey"></param>
        /// <returns>RecoveryID for the recovery context</returns>
        internal static IUserRecoveryContext CreateRecoveryContext(long UserID, string RecoveryKey, bool IsTemporary, bool IsOptional)
        {
            DateTimeOffset resetExpiration;

            if (IsTemporary)
            {
                var span = CoreFactory.Singleton.Properties.AcctPswdRecoveryLifespan;
                if (span.Ticks == 0)
                {
                    resetExpiration = DateTimeOffset.MaxValue;
                }
                else
                {
                    resetExpiration = DateTimeOffset.Now.Add(span);
                }
            }
            else
            {
                if (IsOptional)
                {
                    resetExpiration = DateTimeOffset.Now.AddMonths(1);
                }
                else
                {
                    resetExpiration = DateTimeOffset.MaxValue.Date;
                }
            }

            try
            {
                return SqlWorker.ExecBasicQuery<UserRecoveryContext>(
                    _dbConn,
                    "[dbo].[User_CreateRecoveryContext]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = HandleParamEmpty(UserID);
                        cmd.Parameters.Add("@RecoveryKey", SqlDbType.NVarChar).Value = RecoveryKey;
                        cmd.Parameters.Add("@EffToDate", SqlDbType.DateTimeOffset).Value = resetExpiration;
                        cmd.Parameters.Add("@IsOptional", SqlDbType.Bit).Value = IsOptional;
                    })
                    .SingleOrDefault();
            }
            catch (Exception ex)
            {
                var errCode = "2CE3A9C1-DFC0-4AD0-B0F0-B893DAD61695";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }

        }

        internal static void LogFailedRecoveryContextAttempt(string RecoveryID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_LogFailedRecoveryContextAttempt]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    });
            }
            catch (Exception ex)
            {
                var errCode = "7BF7BD80-7A27-4DEC-9AE1-46FEF34F93FD";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }
        }

        internal static void DeleteRecoveryContext(string RecoveryID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_DeleteUserRecoveryContextByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    });
            }
            catch (Exception ex)
            {
                var errCode = "5A5DCF9F-2C25-4539-9F74-C7BC99EA192D";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }
        }

        internal static void DeleteRecoveryContext(int UserID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_DeleteUserRecoveryContextByUserID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });
            }
            catch (Exception ex)
            {
                var errCode = "8932F761-80E0-4960-99D9-B0995D6F2C3A";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }
        }

    }
}
