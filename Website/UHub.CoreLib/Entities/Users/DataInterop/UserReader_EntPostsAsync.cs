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
        public static async Task<bool> TryValidatePostParentAsync(long UserID, long ParentID)
        {
            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
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
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync("E8A7C53B-BC51-4556-A730-16A570952B5D", ex);
                return false;
            }

        }


        public static async Task<IEnumerable<User>> TryGetPostCommentersAsync(long PostID)
        {
            try
            {
                return await SqlWorker.ExecBasicQueryAsync<User>(
                    _dbConn,
                    "[dbo].[Users_GetPostCommenters]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, "E9B4EC13-A376-4DF2-84DC-97B4508418EB");
                return null;
            }
        }

    }
}
