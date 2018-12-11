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
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Entities.Posts.DataInterop
{

    public static partial class PostReader
    {


        /// <summary>
        /// Get DB post full detail by LONG ID
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static async Task<Post> TryGetPostAsync(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                var temp = await SqlWorker.ExecBasicQueryAsync<Post>(
                    _dbConn,
                    "[dbo].[Post_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

                return temp.SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("9ED9915C-F4C5-4CB7-991F-B40ABDD6A965");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }

        }


        /// <summary>
        /// Get all the posts in the DB
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> TryGetAllPostsAsync()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecBasicQueryAsync<Post>(_dbConn, "[dbo].[Posts_GetAll]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("A273171F-BBF0-4A01-9187-17AF7BA3FEA4");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get set of clubs with associated post counts
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<PostClusteredCount>> TryGetPostClusteredCountsAsync()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecBasicQueryAsync<PostClusteredCount>(_dbConn, "[dbo].[Posts_GetClusteredCounts]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("556303EA-E9D0-46CE-945C-F702C4A9315F");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }




    }
}
