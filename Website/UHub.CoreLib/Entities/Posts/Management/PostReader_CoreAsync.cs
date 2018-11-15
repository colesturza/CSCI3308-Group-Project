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

namespace UHub.CoreLib.Entities.Posts.Management
{

    public static partial class PostReader
    {

        #region Individual
        /// <summary>
        /// Get DB post full detail by LONG ID
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static async Task<Post> GetPostAsync(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var temp = await SqlWorker.ExecBasicQueryAsync<Post>(
                _dbConn,
                "[dbo].[Post_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                });


            return temp.SingleOrDefault();
        }
        #endregion Individual


        #region Group
        /// <summary>
        /// Get all the posts in the DB
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> GetAllPostsAsync()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<Post>(_dbConn, "[dbo].[Posts_GetAll]");
        }


        /// <summary>
        /// Get all the posts in the DB by parent
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> GetPostsByParentAsync(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<Post>(
                _dbConn,
                "[dbo].[Posts_GetByParent]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                });
        }

        /// <summary>
        /// Get all the posts in the DB by school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> GetPostsBySchoolAsync(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<Post>(
                _dbConn,
                "[dbo].[Posts_GetBySchool]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                });
        }

        /// <summary>
        /// Get all the posts in the DB by school club
        /// </summary>
        /// <param name="SchoolClubID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> GetPostsByClubAsync(long SchoolClubID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<Post>(
                _dbConn,
                "[dbo].[Posts_GetByClub]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                });
        }


        #endregion Group


        #region Counters

        public static async Task<IEnumerable<PostClusteredCount>> GetPostClusteredCountsAsync()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<PostClusteredCount>(
                _dbConn,
                "[dbo].[Posts_GetClusteredCounts]",
                (cmd) => { });
        }


        public static async Task<IEnumerable<PostClusteredCount>> GetPostClusteredCountsAsync(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<PostClusteredCount>(
                _dbConn,
                "[dbo].[Posts_GetClusteredCountsBySchool]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                });
        }

        #endregion Counters
    }
}
