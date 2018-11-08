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
using UHub.CoreLib.Entities.Users.Interfaces;
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Entities.Users.Management
{
    public static partial class UserReader
    {


        #region Individual
        public static async Task<bool> DoesUserExistAsync(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[User_DoesExistByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });


            }
            catch (Exception ex)
            {
                var errCode = "A2DEF2E8-2FCE-4374-B15F-47F68EF80995";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        public static async Task<bool> DoesUserExistAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[User_DoesExistByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                });
            }
            catch (Exception ex)
            {
                var errCode = "F23FFB10-4DD5-43FA-910F-D745C431FE1F";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        public static async Task<bool> DoesUserExistAsync(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[User_DoesExistByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                });


            }
            catch (Exception ex)
            {
                var errCode = "77AD516E-109E-47E4-9409-7F61C43674DF";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }



        /// <summary>
        /// Get user ID from email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<long?> GetUserIDAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecScalarAsync<long>(
                _dbConn,
                "[dbo].[User_GetIDByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                });
            }
            catch (Exception ex)
            {
                var errCode = "8E5E8203-137C-4B79-B902-3644EEA887DF";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }

        /// <summary>
        /// Get user ID from username and domain
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public static async Task<long?> GetUserIDAsync(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecScalarAsync<long>(
                _dbConn,
                "[dbo].[User_GetIDByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                });
            }
            catch (Exception ex)
            {
                var errCode = "3E11FF4A-4611-4EB8-A084-CA14040089A4";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }

        /// <summary>
        /// Get DB User full detail by ID
        /// </summary>
        /// <param name="UserUID"></param>
        /// <returns></returns>
        public static async Task<User> GetUserAsync(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                var temp = await SqlWorker.ExecBasicQueryAsync(
                    _dbConn,
                    "[dbo].[User_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    },
                    (reader) =>
                    {
                        return reader.ToCustomDBType<User>();
                    });


                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                var errCode = "A5507A7A-2D24-4433-BCC5-02EF4FF6B374";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }

        /// <summary>
        /// Get DB User full detail by email
        /// </summary>
        ///<param name="Email"
        /// <returns></returns>
        public static async Task<User> GetUserAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                var temp = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[User_GetByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<User>();
                });

                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                var errCode = "AFCC4016-826B-4EB3-9AB9-0ECA77B45784";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }

        /// <summary>
        /// Get DB User full detail by username and domain
        /// </summary>
        ///<param name="Username"
        /// <returns></returns>
        public static async Task<User> GetUserAsync(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                var temp = await SqlWorker.ExecBasicQueryAsync(
                    _dbConn,
                    "[dbo].[User_GetByUsername]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                    },
                    (reader) =>
                    {
                        return reader.ToCustomDBType<User>();
                    });

                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                var errCode = "A7527D36-FAEE-4630-87DB-4CF13A3EF0B2";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }
        #endregion Individual


        #region Group
        /// <summary>
        /// Get users with lower security class
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<User>> GetUsersAsync()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return await SqlWorker.ExecBasicQueryAsync(
                    _dbConn,
                    "[dbo].[User_GetAll]",
                    (cmd) => { },
                    (row) =>
                    {
                        return row.ToCustomDBType<User>();
                    });

            }
            catch (Exception ex)
            {
                var errCode = "86F10DBA-EE88-414C-B8E5-AADEBB6A7977";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }
        #endregion Group




    }
}
