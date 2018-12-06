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
        public static Post TryGetPost(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecBasicQuery<Post>(
                    _dbConn,
                    "[dbo].[Post_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog("CCE3A0A5-DC88-42B6-ADC0-F2F95995203C", ex);
                return null;
            }
        }



        /// <summary>
        /// Get all the posts in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetAllPosts()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecBasicQuery<Post>(_dbConn, "[dbo].[Posts_GetAll]");

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog("00877A2B-2DE3-492A-9EFE-5FDB837E9C59", ex);
                return Enumerable.Empty<Post>();
            }
        }




        /// <summary>
        /// Get set of clubs with associated post counts
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PostClusteredCount> TryGetPostClusteredCounts()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return SqlWorker.ExecBasicQuery<PostClusteredCount>(_dbConn, "[dbo].[Posts_GetClusteredCounts]");

            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog("D93AA28B-A453-4BC1-B32C-882BEAEDCEDE", ex);
                return null;
            }
        }

    }
}
