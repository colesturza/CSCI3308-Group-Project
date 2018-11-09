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

            var temp = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Post_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<Post>();
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


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetAll]",
                (cmd) => { },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });
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


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetByParent]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
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


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetBySchool]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
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


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetByClub]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });
        }


        #endregion Group
    }
}
