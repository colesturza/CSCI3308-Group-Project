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

        public static IUserConfirmToken GetConfirmToken(long UserID)
        {
            try
            {
                return SqlWorker.ExecBasicQuery<UserConfirmToken>(
                    _dbConn,
                    "[dbo].[User_GetConfirmTokenByUserID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                throw new Exception();
            }
        }

        public static IUserConfirmToken GetConfirmToken(string RefUID)
        {
            try
            {
                return SqlWorker.ExecBasicQuery<UserConfirmToken>(
                    _dbConn,
                    "[dbo].[User_GetConfirmTokenByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RefUID", SqlDbType.NVarChar).Value = RefUID;
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
