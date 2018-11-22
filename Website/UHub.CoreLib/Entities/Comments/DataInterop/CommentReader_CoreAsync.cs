using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Comments.DataInterop
{
    public static partial class CommentReader
    {

        /// <summary>
        /// Get all the comments in the DB from a post
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Comment>> TryGetCommentsByPostAsync(long PostID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return await SqlWorker.ExecBasicQueryAsync<Comment>(
                    _dbConn,
                    "[dbo].[Comments_GetByPost]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("66C367D8-119F-4539-92DD-48F01738BDA1", ex);
                return null;
            }
        }

        /// <summary>
        /// Get all the comments in the DB from a single parent
        /// Will only search one level deep based on their direct parent
        /// </summary>
        /// /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Comment>> TryGetCommentsByParentAsync(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return await SqlWorker.ExecBasicQueryAsync<Comment>(
                    _dbConn,
                    "[dbo].[Comments_GetByParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    });

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("63E18CDB-F9A3-41EE-AF75-954AD55A4E40", ex);
                return null;
            }
        }
    }
}
