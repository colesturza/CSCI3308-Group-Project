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
        /// Check if user is able to comment to specified ent parent
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static bool TryValidateCommentParent(long UserID, long ParentID)
        {

            try
            {
                return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[User_ValidateCommentParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = UserID;
                    });

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog("F750F68E-728C-49E7-BA99-3DB3E1CBDB8E", ex);
                return false;
            }

        }


    }
}
