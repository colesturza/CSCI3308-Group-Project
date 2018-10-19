﻿using System;
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


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[Post_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<Post>();
                }).SingleOrDefault();
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


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[Posts_GetAll]",
                (cmd) => { },
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
        public static IEnumerable<Post> GetPostsBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[Posts_GetBySchool]",
                (cmd) => {
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
        public static IEnumerable<Post> GetPostsBySchoolClub(long SchoolClubID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[Posts_GetBySchoolClub]",
                (cmd) => {
                    cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
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


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[Posts_GetBySchoolPage]",
                (cmd) => {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleDBNull(StartID);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleDBNull(PageNum);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
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


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[Posts_GetByClubPage]",
                (cmd) => {
                    cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                    cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleDBNull(StartID);
                    cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleDBNull(PageNum);
                    cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                },
                (row) =>
                {
                    return row.ToCustomDBType<Post>();
                });
        }


        #endregion Group
    }
}