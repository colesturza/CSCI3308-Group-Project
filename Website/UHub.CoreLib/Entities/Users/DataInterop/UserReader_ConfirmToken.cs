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

        public static IUserConfirmToken GetConfirmToken(long UserID)
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


        public static IUserConfirmToken GetConfirmToken(string RefUID)
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

    }
}
