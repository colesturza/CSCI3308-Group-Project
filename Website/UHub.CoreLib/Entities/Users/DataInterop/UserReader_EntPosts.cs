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
        public static bool TryValidatePostParent(long UserID, long ParentID)
        {

            try
            {
                return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[User_ValidatePostParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    });

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog("586E0FC0-904C-4133-ADE7-DD202F6A43D6", ex);
                return false;
            }
        }



        public static IEnumerable<User> TryGetPostCommenters(long PostID)
        {
            try
            {
                return SqlWorker.ExecBasicQuery<User>(
                    _dbConn,
                    "[dbo].[Users_GetPostCommenters]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog("C21265BA-9169-4F85-A2AB-72151DFB896F", ex);
                return null;
            }
        }


    }
}
