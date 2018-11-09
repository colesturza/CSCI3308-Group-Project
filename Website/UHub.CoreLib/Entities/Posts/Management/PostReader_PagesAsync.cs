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
        public static async Task<long> GetPostCountByParentAsync(long ParentID, bool IncludePrivatePosts)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return await SqlWorker.ExecScalarAsync<long>(
                _dbConn,
                "[dbo].[Posts_GetCountByParent]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    cmd.Parameters.Add("@IncludePrivatePosts", SqlDbType.Bit).Value = IncludePrivatePosts;
                });

        }


        /// <summary>
        /// Get all the posts in the DB by parent page
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> GetPostsByParentPageAsync(long ParentID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            //SET DEFAULT ITEM COUNT
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetByParentPage]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(StartID);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(PageNum);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });
        }


        /// <summary>
        /// Get all the posts in the DB by parent page.  Uses default StartID/PageNum of 0
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<Post> PostSet, long StartID)> GetPostsByParentPageAsync(long ParentID, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            var postSetRaw = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetByParentPage]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(null);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(null);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });


            var postSet = postSetRaw.ToList();


            long maxID = -1;
            for (int i = 0; i < postSet.Count; i++)
            {
                if ((postSet[i]?.ID ?? -1) > maxID)
                {
                    maxID = postSet[i].ID.Value;
                }
            }


            return (postSet, maxID);
        }



        public static async Task<long> GetPostCountBySchoolAsync(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return await SqlWorker.ExecScalarAsync<long>(
                _dbConn,
                "[dbo].[Posts_GetCountBySchool]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                });

        }


        /// <summary>
        /// Get all the posts in the DB by school page
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> GetPostsBySchoolPageAsync(long SchoolID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            //SET DEFAULT ITEM COUNT
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetBySchoolPage]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(StartID);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(PageNum);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });
        }


        /// <summary>
        /// Get all the posts in the DB by school page.  Uses default StartID/PageNum of 0
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<Post> PostSet, long StartID)> GetPostsBySchoolPageAsync(long SchoolID, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            var postSetRaw = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetBySchoolPage]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(null);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(null);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });


            var postSet = postSetRaw.ToList();


            long maxID = -1;
            for (int i = 0; i < postSet.Count; i++)
            {
                if ((postSet[i]?.ID ?? -1) > maxID)
                {
                    maxID = postSet[i].ID.Value;
                }
            }


            return (postSet, maxID);
        }


        public static async Task<long> GetPostCountByClubAsync(long ClubID, bool IncludePrivatePosts)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return await SqlWorker.ExecScalarAsync<long>(
                _dbConn,
                "[dbo].[Posts_GetCountByClub]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                    cmd.Parameters.Add("@IncludePrivatePosts", SqlDbType.Bit).Value = IncludePrivatePosts;
                });

        }

        /// <summary>
        /// Get all the posts in the DB by club page
        /// </summary>
        /// <param name="ClubID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> GetPostsByClubPageAsync(long ClubID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            return await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetByClubPage]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(StartID);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(PageNum);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });
        }


        /// <summary>
        /// Get all the posts in the DB by club page.  Uses default StartID/PageNum of 0
        /// </summary>
        /// <param name="ClubID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<(List<Post> PostSet, long StartID)> GetPostsByClubPageAsync(long ClubID, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            var postSetRaw = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[Posts_GetByClubPage]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(null);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(null);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });


            var postSet = postSetRaw.ToList();

            long maxID = -1;
            for (int i = 0; i < postSet.Count; i++)
            {
                if ((postSet[i]?.ID ?? -1) > maxID)
                {
                    maxID = postSet[i].ID.Value;
                }
            }


            return (postSet, maxID);


        }

    }
}
