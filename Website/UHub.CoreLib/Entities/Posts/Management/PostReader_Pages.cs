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
        public static long GetPostCountByParent(long ParentID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return SqlWorker.ExecScalar<long>(
                _dbConn,
                "[dbo].[Posts_GetCountByParent]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
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
        public static IEnumerable<Post> GetPostsByParentPage(long ParentID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            //SET DEFAULT ITEM COUNT
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            return SqlWorker.ExecBasicQuery(
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
        public static List<Post> GetPostsByParentPage(long ParentID, short? ItemCount, out long StartID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            var postSet = SqlWorker.ExecBasicQuery(
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
                }).ToList();


            long maxID = -1;
            for (int i = 0; i < postSet.Count; i++)
            {
                if ((postSet[i]?.ID ?? -1) > maxID)
                {
                    maxID = postSet[i].ID.Value;
                }
            }
            StartID = maxID;


            return postSet;
        }



        public static long GetPostCountBySchool(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return SqlWorker.ExecScalar<long>(
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
        public static IEnumerable<Post> GetPostsBySchoolPage(long SchoolID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            //SET DEFAULT ITEM COUNT
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            return SqlWorker.ExecBasicQuery(
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
        public static List<Post> GetPostsBySchoolPage(long SchoolID, short? ItemCount, out long StartID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            var postSet = SqlWorker.ExecBasicQuery(
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
                }).ToList();


            long maxID = -1;
            for (int i = 0; i < postSet.Count; i++)
            {
                if ((postSet[i]?.ID ?? -1) > maxID)
                {
                    maxID = postSet[i].ID.Value;
                }
            }
            StartID = maxID;


            return postSet;
        }


        public static long GetPostCountByClub(long ClubID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return SqlWorker.ExecScalar<long>(
                _dbConn,
                "[dbo].[Posts_GetCountByClub]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
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
        public static IEnumerable<Post> GetPostsByClubPage(long ClubID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            return SqlWorker.ExecBasicQuery(
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
        public static List<Post> GetPostsByClubPage(long ClubID, short? ItemCount, out long StartID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            var postSet = SqlWorker.ExecBasicQuery(
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
                }).ToList();



            long maxID = -1;
            for (int i = 0; i < postSet.Count; i++)
            {
                if ((postSet[i]?.ID ?? -1) > maxID)
                {
                    maxID = postSet[i].ID.Value;
                }
            }
            StartID = maxID;


            return postSet;


        }

    }
}
