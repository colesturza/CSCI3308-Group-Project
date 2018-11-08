using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Entities.Users.Management
{
    public static partial class UserReader
    {
        /// <summary>
        /// Check if user is able to comment to specified ent parent
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<bool> ValidateCommentParentAsync(long UserID, long ParentID)
        {

            return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[User_ValidateCommentParent]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = UserID;
                });

        }


    }
}
