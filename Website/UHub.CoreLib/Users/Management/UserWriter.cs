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
using UHub.CoreLib.Users.Interfaces;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.Users.Management
{
    internal static partial class UserWriter
    {


        /// <summary>
        /// Attempts to create a new CMS user in the database and returns the UserUID if successful
        /// </summary>
        /// <param name="FriendlyID"></param>
        /// <param name="IsConfirmed"></param>
        /// <param name="IsApproved"></param>
        /// <param name="IsEnabled"></param>
        /// <param name="ModifiedBy"></param>
        /// <returns></returns>
        internal static long? TryCreateUser(IUser_Create_Internal cmsUser)
        {
            return TryCreateUser(cmsUser, out _);

        }

        /// <summary>
        /// Attempts to create a new CMS user in the database and returns the UserUID if successful
        /// </summary>
        /// <param name="FriendlyID"></param>
        /// <param name="IsConfirmed"></param>
        /// <param name="IsApproved"></param>
        /// <param name="IsEnabled"></param>
        /// <param name="ModifiedBy"></param>
        /// <returns></returns>
        internal static long? TryCreateUser(IUser_Create_Internal cmsUser, out string ErrorMsg)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                long? userID = SqlWorker.ExecScalar<long?>
                    (CoreFactory.Singleton.Properties.CmsDBConfig,
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
                    throw new Exception(ResponseStrings.DBError.WRITE_UNKNOWN);
                }

                ErrorMsg = null;
                return userID;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                ErrorMsg = ex.Message;
                return null;
            }
        }

        internal static void UpdateUserInfo(IUser_Update_Public cmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (cmsUser.UserID == null)
            {
                throw new ArgumentException("Invalid target user");
            }


            try
            {

                //run sproc
                SqlWorker.ExecNonQuery
                (CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdateByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = HandleParamEmpty(cmsUser.UserID);
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
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                throw new Exception();
            }
        }

        /// <summary>
        /// Confirm user account
        /// </summary>
        /// <param name="RefUID"></param>
        internal static bool ConfirmUser(string RefUID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecScalar<bool>(
                        CoreFactory.Singleton.Properties.CmsDBConfig,
                        "[dbo].[User_UpdateConfirmFlag]",
                        (cmd) =>
                        {
                            cmd.Parameters.Add("@RefUID", SqlDbType.NVarChar).Value = RefUID;
                        });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
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
                SqlWorker.ExecNonQuery
                (CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdateApprovalFlag]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserUID;
                    cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = IsApproved;
                });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
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
                SqlWorker.ExecNonQuery
                (CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdateVersionByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@Version", SqlDbType.NVarChar).Value = Version;
                });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
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
                SqlWorker.ExecNonQuery
                (CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_DeleteByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@DeletedBy", SqlDbType.BigInt).Value = DeletedBy;
                });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                throw new Exception();
            }
        }
        /// <summary>
        /// Attempt to purge a user from the DB
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="UserUID"></param>
        /// <returns></returns>
        internal static bool TryPurgeUser(long UserID)
        {
            try
            {
                SqlWorker.ExecNonQuery
                (CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_PurgeByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });

                return true;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                return false;
            }
        }

        /// <summary>
        /// Attempt to purge users that have not yet been confirmed and are older than the specified age tolerance
        /// </summary>
        internal static void TryPurgeUnconfirmedUsers(TimeSpan AcctAgeTolerance)
        {
            //users created before this date are subject to be purged
            //users created afgter this date will be ignored
            var minKeepDate = DateTimeOffset.UtcNow - AcctAgeTolerance;

            SqlWorker.ExecNonQuery
                (CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[Users_PurgeUnconfirmed]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@MinKeepDate", SqlDbType.UniqueIdentifier).Value = minKeepDate;
                });
        }


        #region Recovery
        /// <summary>
        /// Create an account recovery context for a specified user
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="RecoveryKey"></param>
        /// <returns>RecoveryID for the recovery context</returns>
        internal static IUserRecoveryContext CreateRecoveryContext(long UserID, string RecoveryKey, bool IsTemporary, bool IsOptional)
        {
            DateTimeOffset resetExpiration;

            if (IsTemporary)
            {
                var span = CoreFactory.Singleton.Properties.AcctPswdResetExpiration;
                if (span.Ticks == 0)
                {
                    resetExpiration = DateTimeOffset.MaxValue;
                }
                else
                {
                    resetExpiration = DateTimeOffset.Now.Add(span);
                }
            }
            else
            {
                if (IsOptional)
                {
                    resetExpiration = DateTimeOffset.Now.AddMonths(1);
                }
                else
                {
                    resetExpiration = DateTimeOffset.MaxValue.Date;
                }
            }

            try
            {
                return SqlWorker.ExecBasicQuery(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_CreateRecoveryContext]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = HandleParamEmpty(UserID);
                        cmd.Parameters.Add("@RecoveryKey", SqlDbType.NVarChar).Value = RecoveryKey;
                        cmd.Parameters.Add("@EffToDate", SqlDbType.DateTimeOffset).Value = resetExpiration;
                        cmd.Parameters.Add("@IsOptional", SqlDbType.Bit).Value = IsOptional;
                    }, (reader) =>
                    {
                        var output = reader.ToCustomDBType<UserRecoveryContext>();
                        return output;
                    }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                throw new Exception();
            }

        }

        internal static void LogFailedRecoveryContextAttempt(string RecoveryID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_LogFailedRecoveryContextAttempt]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                throw new Exception();
            }
        }

        internal static void DeleteRecoveryContext(string RecoveryID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_DeleteUserRecoveryContextByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                throw new Exception();
            }
        }

        internal static void DeleteRecoveryContext(int UserID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_DeleteUserRecoveryContextByUserID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                throw new Exception();
            }
        }
        #endregion Recovery


    }
}
