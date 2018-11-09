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

namespace UHub.CoreLib.Entities.Comments.Management
{
    public static partial class CommentReader
    {

        #region Group
        /// <summary>
        /// Get all the comments in the DB from a post
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Comment>> GetCommentsByPostAsync(long PostID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Comments_GetByPost]",
                (cmd) => {
                    cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Comment>();
                });
        }

        /// <summary>
        /// Get all the comments in the DB from a single parent
        /// Will only search one level deep based on their direct parent
        /// </summary>
        /// /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Comment>> GetCommentsByParentAsync(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Comments_GetByParent]",
                (cmd) => {
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Comment>();
                });
        }
        #endregion Group
    }
}
