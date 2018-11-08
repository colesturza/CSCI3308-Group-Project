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

namespace UHub.CoreLib.Entities.Users.Management
{
    internal static partial class UserWriter
    {

        /// <summary>
        /// Attempts to create a new CMS user in the database and returns the UserUID if successful
        /// </summary>
        /// <returns></returns>
        internal static async Task<long?> TryCreateUserAsync(User cmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                long? userID = await SqlWorker.ExecScalarAsync<long?>
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
                    return null;
                }

                return userID;
            }
            catch (Exception ex)
            {
                var errCode = "0E94B3A8-CBDA-4EA5-8DDB-1C50D8496763";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);


                return null;
            }
        }

        internal static async Task<bool> TryUpdateUserInfoAsync(User cmsUser)
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
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
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


                return true;
            }
            catch (Exception ex)
            {
                var errCode = "9E176176-3FE8-4739-B071-960647EA2193";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        /// <summary>
        /// Confirm user account
        /// </summary>
        /// <param name="RefUID"></param>
        internal static async Task<bool> ConfirmUserAsync(string RefUID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
                        CoreFactory.Singleton.Properties.CmsDBConfig,
                        "[dbo].[User_UpdateConfirmFlag]",
                        (cmd) =>
                        {
                            cmd.Parameters.Add("@RefUID", SqlDbType.NVarChar).Value = RefUID;
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
        internal static async Task<bool> TryUpdateUserApprovalAsync(long UserUID, bool IsApproved)
        {
            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_UpdateApprovalFlag]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserUID;
                        cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = IsApproved;
                    });


                return true;
            }
            catch (Exception ex)
            {
                var errCode = "0EF7E744-5F24-4EB2-9CD5-CF8C604976D9";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        /// <summary>
        /// Update the User's ticket version to invalidate existing login tickets
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="Version"></param>
        internal static async Task<bool> TryUpdateUserVersionAsync(long UserID, string Version)
        {

            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_UpdateVersionByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@Version", SqlDbType.NVarChar).Value = Version;
                    });


                return true;
            }
            catch (Exception ex)
            {
                var errCode = "78485CFC-5709-49EE-BBB4-91A3A9D4B625";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }


        /// <summary>
        /// Delete a specified user from the CMS DB
        /// </summary>
        /// <param name="RequestedBy">The user executing the delete command</param>
        /// <param name="UserUID">The user being deleted</param>
        internal static async Task<bool> DeleteUserAsync(long UserID, long? DeletedBy = null)
        {
            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_DeleteByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@DeletedBy", SqlDbType.BigInt).Value = DeletedBy;
                    });


                return true;
            }
            catch (Exception ex)
            {
                var errCode = "017BDF75-40BA-4F89-B15C-5EB2CEFFC7E5";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }
        /// <summary>
        /// Attempt to purge a user from the DB
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        internal static async Task<bool> TryPurgeUserAsync(long UserID)
        {
            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
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
        internal static async Task<bool> TryPurgeUserAsync(string Email)
        {
            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
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
        internal static async Task<bool> TryPurgeUnconfirmedUsersAsync(TimeSpan AcctAgeTolerance)
        {
            try
            {
                //users created before this date are subject to be purged
                //users created afgter this date will be ignored
                var minKeepDate = DateTimeOffset.UtcNow - AcctAgeTolerance;

                await SqlWorker.ExecNonQueryAsync
                    (CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Users_PurgeUnconfirmed]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@MinKeepDate", SqlDbType.UniqueIdentifier).Value = minKeepDate;
                    });


                return true;
            }
            catch (Exception ex)
            {
                var errCode = "6FE73439-372D-4935-92C9-912B47822499";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }


        #region Recovery
        /// <summary>
        /// Create an account recovery context for a specified user
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="RecoveryKey"></param>
        /// <returns>RecoveryID for the recovery context</returns>
        internal static async Task<IUserRecoveryContext> CreateRecoveryContextAsync(long UserID, string RecoveryKey, bool IsTemporary, bool IsOptional)
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
                var temp = await SqlWorker.ExecBasicQueryAsync(
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
                    });


                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                var errCode = "2CE3A9C1-DFC0-4AD0-B0F0-B893DAD61695";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }

        }

        internal static async Task<bool> TryLogFailedRecoveryContextAttemptAsync(string RecoveryID)
        {
            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_LogFailedRecoveryContextAttempt]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    });


                return true;
            }
            catch (Exception ex)
            {
                var errCode = "7BF7BD80-7A27-4DEC-9AE1-46FEF34F93FD";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);


                return false;
            }
        }

        internal static async Task<bool> TryDeleteRecoveryContextAsync(string RecoveryID)
        {
            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_DeleteUserRecoveryContextByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                    });

                return true;
            }
            catch (Exception ex)
            {
                var errCode = "5A5DCF9F-2C25-4539-9F74-C7BC99EA192D";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return false;
            }
        }

        internal static async Task<bool> TryDeleteRecoveryContextAsync(int UserID)
        {
            try
            {
                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_DeleteUserRecoveryContextByUserID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });


                return true;
            }
            catch (Exception ex)
            {
                var errCode = "8932F761-80E0-4960-99D9-B0995D6F2C3A";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);


                return false;
            }
        }
        #endregion Recovery


    }
}
