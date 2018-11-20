using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    [Obsolete("Should not be used directly.  Use AccountManager instead.")]
    internal static partial class UserWriter
    {
        /// <summary>
        /// Attempts to create a new CMS user in the database and returns the UserUID if successful
        /// </summary>
        /// <returns></returns>
        internal static long? TryCreateUser(User cmsUser) => TryCreateUser(cmsUser, out _);

        /// <summary>
        /// Attempts to create a new CMS user in the database and returns the UserUID if successful
        /// </summary>
        /// <returns></returns>
        internal static long? TryCreateUser(User cmsUser, out string ErrorMsg)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                long? userID = SqlWorker.ExecScalar<long?>(
                    _dbConn,
                    "[dbo].[User_Create]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = HandleParamEmpty(cmsUser.SchoolID);
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Email);
                        cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = HandleParamEmpty(cmsUser.Username);
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Name);
                        cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.PhoneNumber);
                        cmd.Parameters.Add("@Major", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Major);
                        cmd.Parameters.Add("@Year", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Year);
                        cmd.Parameters.Add("@GradDate", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.GradDate);
                        cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Company);
                        cmd.Parameters.Add("@JobTitle", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.JobTitle);
                        cmd.Parameters.Add("@IsFinished", SqlDbType.NVarChar).Value = DBNull.Value;
                        cmd.Parameters.Add("@Version", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Version);
                        cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = HandleParamEmpty(cmsUser.IsApproved);
                        cmd.Parameters.Add("@IsConfirmed", SqlDbType.Bit).Value = HandleParamEmpty(cmsUser.IsConfirmed);
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = DBNull.Value;
                    });

                if (userID == null)
                {
                    ErrorMsg = ResponseStrings.DBError.WRITE_UNKNOWN;
                    return null;
                }

                ErrorMsg = null;
                return userID;
            }
            catch (Exception ex)
            {
                var errCode = "0E94B3A8-CBDA-4EA5-8DDB-1C50D8496763";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                ErrorMsg = ex.Message;
                return null;
            }
        }

        internal static void UpdateUserInfo(User cmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (cmsUser.ID == null)
            {
                throw new ArgumentException("Invalid target user");
            }


            try
            {

                //run sproc
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_UpdateByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = HandleParamEmpty(cmsUser.ID);
                        //-------------------------------------------------
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Name);
                        cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.PhoneNumber);
                        cmd.Parameters.Add("@Major", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Major);
                        cmd.Parameters.Add("@Year", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Year);
                        cmd.Parameters.Add("@GradDate", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.GradDate);
                        cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Company);
                        cmd.Parameters.Add("@JobTitle", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.JobTitle);
                        //-------------------------------------------------
                        cmd.Parameters.Add("@ModifiedBy", SqlDbType.BigInt).Value = DBNull.Value;
                    });

            }
            catch (Exception ex)
            {
                var errCode = "9E176176-3FE8-4739-B071-960647EA2193";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }
        }

        /// <summary>
        /// Confirm user account
        /// </summary>
        /// <param name="RefUID"></param>
        /// <param name="MinCreatedDate">If the confirmation token is create before this time, then it is expired</param>
        internal static bool ConfirmUser(string RefUID, DateTimeOffset MinCreatedDate)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecScalar<bool>(
                        _dbConn,
                        "[dbo].[User_UpdateConfirmFlag]",
                        (cmd) =>
                        {
                            cmd.Parameters.Add("@RefUID", SqlDbType.NVarChar).Value = RefUID;
                            cmd.Parameters.Add("@MinCreatedDate", SqlDbType.DateTimeOffset).Value = MinCreatedDate;
                        });
            }
            catch (Exception ex)
            {
                var errCode = "65F803F1-5C9E-41AD-84F3-B7CCF6C47873";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        /// <summary>
        /// Update user approval status
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="IsApproved"></param>
        internal static void UpdateUserApproval(long UserUID, bool IsApproved)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_UpdateApprovalFlag]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserUID;
                        cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = IsApproved;
                    });
            }
            catch (Exception ex)
            {
                var errCode = "0EF7E744-5F24-4EB2-9CD5-CF8C604976D9";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }
        }

        /// <summary>
        /// Update the User's ticket version to invalidate existing login tickets
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="Version"></param>
        internal static void UpdateUserVersion(long UserID, string Version)
        {

            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_UpdateVersionByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@Version", SqlDbType.NVarChar).Value = Version;
                    });
            }
            catch (Exception ex)
            {
                var errCode = "78485CFC-5709-49EE-BBB4-91A3A9D4B625";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }
        }


        /// <summary>
        /// Delete a specified user from the CMS DB
        /// </summary>
        /// <param name="RequestedBy">The user executing the delete command</param>
        /// <param name="UserUID">The user being deleted</param>
        internal static void DeleteUser(long UserID, long? DeletedBy = null)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_DeleteByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@DeletedBy", SqlDbType.BigInt).Value = DeletedBy;
                    });
            }
            catch (Exception ex)
            {
                var errCode = "017BDF75-40BA-4F89-B15C-5EB2CEFFC7E5";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                throw new Exception();
            }
        }
        /// <summary>
        /// Attempt to purge a user from the DB
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        internal static bool TryPurgeUser(long UserID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_PurgeByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });

                return true;
            }
            catch (Exception ex)
            {
                var errCode = "84986584-BD1D-4DDB-8DAA-475A3BB874C1";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        /// <summary>
        /// Attempt to purge a user from the DB
        /// </summary>
        /// <returns></returns>
        internal static bool TryPurgeUser(string Email)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_PurgeByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    });

                return true;
            }
            catch (Exception ex)
            {
                var errCode = "CDFB440C-2271-4DB4-BDE8-FC198D1FDACC";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        /// <summary>
        /// Attempt to purge users that have not yet been confirmed and are older than the specified age tolerance
        /// </summary>
        internal static void TryPurgeUnconfirmedUsers(TimeSpan AcctAgeTolerance)
        {
            try
            {
                //users created before this date are subject to be purged
                //users created afgter this date will be ignored
                var minKeepDate = DateTimeOffset.UtcNow - AcctAgeTolerance;

                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[Users_PurgeUnconfirmed]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@MinKeepDate", SqlDbType.UniqueIdentifier).Value = minKeepDate;
                    });
            }
            catch (Exception ex)
            {
                var errCode = "6FE73439-372D-4935-92C9-912B47822499";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
            }
        }

    }
}
