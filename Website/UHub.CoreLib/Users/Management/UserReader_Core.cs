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
using UHub.CoreLib.Users.Interfaces;

namespace UHub.CoreLib.Users.Management
{
    public static partial class UserReader
    {
        private static string _dbConn = null;

        static UserReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

        public static bool DoesUserExist(long UserID)
        {
            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                "[dbo].[User_DoesExistByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }

        public static bool DoesUserExist(string UserRef, UserRefType RefType)
        {
            string sprocName = null;
            string paramName = null;

            if (RefType == UserRefType.Email)
            {
                sprocName = "[dbo].[User_DoesExistByEmail]";
                paramName = "@Email";
            }
            else if (RefType == UserRefType.Username)
            {
                sprocName = "[dbo].[User_DoesExistByUsername]";
                paramName = "@Username";
            }

            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                sprocName,
                (cmd) =>
                {
                    cmd.Parameters.Add(paramName, SqlDbType.NVarChar).Value = SqlConverters.HandleParamEmpty(UserRef);
                });
        }


        #region Individual
        /// <summary>
        /// Get user ID from email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static long? GetUserID(string UserRef, UserRefType RefType)
        {
            string sprocName = null;
            string paramName = null;

            if (RefType == UserRefType.Email)
            {
                sprocName = "[dbo].[User_GetIDByEmail]";
                paramName = "@Email";
            }
            else if (RefType == UserRefType.Username)
            {
                sprocName = "[dbo].[User_GetIDByUsername]";
                paramName = "@Username";
            }


            return SqlWorker.ExecScalar<long>(
                _dbConn,
                sprocName,
                (cmd) =>
                {
                    cmd.Parameters.Add(paramName, SqlDbType.NVarChar).Value = UserRef;
                });
        }

        /// <summary>
        /// Get DB User full detail by GUID UID
        /// </summary>
        /// <param name="UserUID"></param>
        /// <returns></returns>
        public static IUser_Internal GetUser(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[User_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<User>();
                }).SingleOrDefault();
        }

        /// <summary>
        /// Get DB User full detail by GUID UID
        /// </summary>
        /// <param name="UserUID"></param>
        /// <returns></returns>
        internal static IUser_Internal GetUser(string UserRef, UserRefType RefType)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            string sprocName = null;
            string paramName = null;
            if (RefType == UserRefType.Email)
            {
                sprocName = "[dbo].[User_GetByEmail]";
                paramName = "@Email";
            }
            else if (RefType == UserRefType.Username)
            {
                sprocName = "[dbo].[User_GetByUsername]";
                paramName = "@Username";
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                sprocName,
                (cmd) =>
                {
                    cmd.Parameters.Add(paramName, SqlDbType.NVarChar).Value = UserRef;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<User>();
                }).SingleOrDefault();
        }
        #endregion Individual


        #region Group
        /// <summary>
        /// Get users with lower security class
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IUser_Internal> GetUsers()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[User_GetAll]",
                (cmd) => { },
                (row) =>
                {
                    return row.ToCustomDBType<User>();
                });
        }
        #endregion Group


        #region System
        private static long GetUserSystemID()
        {
            //HARDCODE
            //CONSTANT
            return Common.SYSTEM_USER_ID;
        }

        #endregion System


    }
}
