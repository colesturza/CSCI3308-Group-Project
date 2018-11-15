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
        //TODO: abstract to config file
        private const short DEFAULT_PAGE_SIZE = 20;

        private static string _dbConn = null;

        static PostReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

        #region Individual
        /// <summary>
        /// Get DB post full detail by LONG ID
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static Post GetPost(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<Post>(
                _dbConn,
                "[dbo].[Post_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                })
                .SingleOrDefault();
        }
        #endregion Individual

        #region Group
        /// <summary>
        /// Get all the posts in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Post> GetAllPosts()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<Post>(
                _dbConn,
                "[dbo].[Posts_GetAll]",
                (cmd) => { });
        }


        /// <summary>
        /// Get all the posts in the DB by parent
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static IEnumerable<Post> GetPostsByParent(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<Post>(
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
        public static IEnumerable<Post> GetPostsBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<Post>(
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
        public static IEnumerable<Post> GetPostsByClub(long SchoolClubID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<Post>(
                _dbConn,
                "[dbo].[Posts_GetByClub]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                });
        }




        #endregion Group

        #region Counters

        public static IEnumerable<PostClusteredCount> GetPostClusteredCounts()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<PostClusteredCount>(
                _dbConn,
                "[dbo].[Posts_GetClusteredCounts]",
                (cmd) => { });
        }

        public static IEnumerable<PostClusteredCount> GetPostClusteredCounts(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<PostClusteredCount>(
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
