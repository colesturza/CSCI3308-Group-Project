using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SysSec = System.Web.Security;
using System.Web;
using RgxPtrn = UHub.CoreLib.Regex.Patterns;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Management;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.SmtpInterop;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Security.Accounts.Interfaces;
using UHub.CoreLib.Security.Authentication;


namespace UHub.CoreLib.Security.Accounts
{
    /// <summary>
    /// Wrapper for UserWriter functionality.  Controls user account create/edit/delete functionality while also adding error callback functionality
    /// </summary>
    public partial class AccountManager : IAccountManager
    {
        /// <summary>
        /// Try to create a new user in the CMS system
        /// </summary>
        /// <param name="UserEmail">New user email</param>
        /// <param name="UserPassword">New user password</param>
        /// <param name="AttemptAutoLogin">Should system automatically login user after creating account</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler">Args: new user object, auto login [T|F]</param>
        /// <returns>Status Flag</returns>
        public async Task<AcctCreateResultCode> TryCreateUserAsync(
            User NewUser,
            bool AttemptAutoLogin,
            Action<Guid> GeneralFailHandler = null,
            Action<User, bool> SuccessHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (NewUser == null)
            {
                return AcctCreateResultCode.UserInvalid;
            }

            var taskDoesEmailExist = UserReader.DoesUserExistAsync(NewUser.Email);
            var taskSchoolMajors = SchoolMajorReader.GetMajorsBySchoolAsync(NewUser.SchoolID.Value);

            //TODO: finalize trims
            //TODO: validate year against list
            Shared.TryCreate_HandleAttrTrim(ref NewUser);


            var attrValidation = Shared.TryCreate_ValidateUserAttrs(NewUser);
            if (attrValidation != 0)
            {
                return attrValidation;
            }

            var pswdValidation = Shared.ValidateUserPswd(NewUser);
            if (pswdValidation != 0)
            {
                return (AcctCreateResultCode)pswdValidation;
            }


            //Validate user domain and school
            var domain = NewUser.Email.GetEmailDomain();
            var taskGetUserSchool = SchoolReader.GetSchoolByDomainAsync(domain);
            var taskDoesUsernameExist = UserReader.DoesUserExistAsync(NewUser.Username, domain);



            //check for duplicate email
            if (await taskDoesEmailExist)
            {
                return AcctCreateResultCode.EmailDuplicate;
            }


            var tmpSchool = await taskGetUserSchool;
            if (tmpSchool == null || tmpSchool.ID == null)
            {
                return AcctCreateResultCode.EmailDomainInvalid;
            }
            NewUser.SchoolID = tmpSchool.ID;


            //check for duplicate username
            if (await taskDoesUsernameExist)
            {
                return AcctCreateResultCode.UsernameDuplicate;
            }


            //check for valid major (chosen via dropdown)
            var major = NewUser.Major;
            var majorValidationSet = (await taskSchoolMajors)
                                        .Select(x => x.Name)
                                        .ToHashSet();


            if (!majorValidationSet.Contains(major))
            {
                return AcctCreateResultCode.MajorInvalid;
            }

            Shared.TryCreate_HandleUserDefaults(ref NewUser);


            try
            {
#pragma warning disable 612, 618
                //create CMS user
                var userID = await UserWriter.TryCreateUserAsync(NewUser);
#pragma warning restore


                if (userID == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("8ED72683-F21D-4E24-9899-473B20782B33"));
                    return AcctCreateResultCode.UnknownError;
                }


                //try to create password
                //if failed, then purge the remaining CMS account components so user can try again


#pragma warning disable 612, 618
                var salt = SysSec.Membership.GeneratePassword(SALT_LEN, 0);
                string pswdHash = null;
                try
                {
                    var hashType = CoreFactory.Singleton.Properties.PswdHashType;
                    pswdHash = NewUser.Password.GetCryptoHash(hashType, salt);
                }
                catch
                {
                    await UserWriter.TryPurgeUserAsync((long)userID);
                    throw new Exception(ResponseStrings.AccountError.ACCOUNT_FAIL);
                }
                if (pswdHash.IsEmpty())
                {
                    await UserWriter.TryPurgeUserAsync((long)userID);
                    throw new Exception(ResponseStrings.AccountError.ACCOUNT_FAIL);
                }
                try
                {
                    //SET DB PASSWORD
                    await UpdateUserPassword_DBAsync((long)userID, pswdHash, salt);
                }
                catch
                {
                    await UserWriter.TryPurgeUserAsync((long)userID);
                    throw new Exception(ResponseStrings.AccountError.ACCOUNT_FAIL);
                }
#pragma warning restore



                //get cms user
                var cmsUser = UserReader.GetUser(userID.Value);
                var taskConfirmToken = UserReader.GetConfirmTokenAsync(userID.Value);


                //attempt autologin
                //autoconfirm user -> auto login
                bool canLogin =
                    AttemptAutoLogin
                    && CoreFactory.Singleton.Properties.AutoConfirmNewAccounts
                    && CoreFactory.Singleton.Properties.AutoApproveNewAccounts;


                if (canLogin)
                {
                    try
                    {
                        //set login
                        CoreFactory.Singleton.Auth.TrySetClientAuthToken(NewUser.Email, NewUser.Password, false);

                        //TODO: log to DB
                        //CoreFactory.Singleton.Logging.CreateDBActivityLog(ActivityLogTypes.UserLogin);
                    }
                    catch
                    {
                        //account creating, but auto login failed
                        //should only ever occur during tests
                        var errCode = "F475C097-FC34-45E8-BEEF-EF37C9BC48B0";
                        CoreFactory.Singleton.Logging.CreateErrorLogAsync(errCode);

                        SuccessHandler?.Invoke(cmsUser, false);
                        return AcctCreateResultCode.Success;
                    }
                }
                else if (!CoreFactory.Singleton.Properties.AutoConfirmNewAccounts)
                {
                    var siteName = CoreFactory.Singleton.Properties.SiteFriendlyName;
                    var confirmToken = await taskConfirmToken;

                    var msg = new SmtpMessage_ConfirmAcct($"{siteName} Account Confirmation", siteName, NewUser.Email)
                    {
                        ConfirmationURL = confirmToken.GetURL()
                    };

                    var emailSendStatus = await CoreFactory.Singleton.Mail.TrySendMessageAsync(msg);
                    if (emailSendStatus != SmtpResultCode.Success)
                    {
                        var errCode = "0ADC52B0-89BB-4346-84F3-1F6CAC63DACF";
                        CoreFactory.Singleton.Logging.CreateErrorLogAsync(errCode);
                        GeneralFailHandler?.Invoke(new Guid(errCode));

                        return AcctCreateResultCode.UnknownError;
                    }
                }


                SuccessHandler?.Invoke(cmsUser, canLogin);
                return AcctCreateResultCode.Success;
            }
            catch (DuplicateNameException)
            {
                return AcctCreateResultCode.EmailDuplicate;
            }
            catch (Exception ex)
            {
                var errCode = "87C1806A-32DE-4077-ABE4-DA08C9493B6D";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return AcctCreateResultCode.UnknownError;
            }
        }



        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AcctConfirmResultCode> TryConfirmUserAsync(string RefUID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (RefUID.IsEmpty())
            {
                return AcctConfirmResultCode.RefUIDEmpty;
            }

            if (!RefUID.RgxIsMatch(RgxPtrn.EntUser.REF_UID_B))
            {
                return AcctConfirmResultCode.RefUIDInvalid;
            }


            //get Today - ConfLifespan 
            //Determine the earliest date that a confirmation email could be created and still be valid
            //If ConfLifespan is 0, then allow infinite time
            DateTimeOffset minCreatedDate = DateTimeOffset.MinValue;
            var confLifespan = CoreFactory.Singleton.Properties.AcctConfirmLifespan;
            if (confLifespan != TimeSpan.Zero)
            {
                minCreatedDate = DateTimeOffset.UtcNow - confLifespan;
            }

            try
            {
#pragma warning disable 612, 618
                var isValid = await UserWriter.ConfirmUserAsync(RefUID, minCreatedDate);
#pragma warning restore


                if (isValid)
                {
                    return AcctConfirmResultCode.Success;
                }
                else
                {
                    return AcctConfirmResultCode.ConfirmTokenInvalid;
                }
            }
            catch
            {
                return AcctConfirmResultCode.UnknownError;
            }
        }


        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        public async Task<bool> TryUpdateUserApprovalStatusAsync(long UserID, bool IsApproved)
        {
#pragma warning disable 612, 618
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                var result = await UserWriter.TryUpdateUserApprovalAsync(UserID, IsApproved);

                return result;
            }
            catch
            {
                return false;
            }
#pragma warning restore
        }


        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public async Task<AcctPswdResultCode> TryUpdatePasswordAsync(
            string UserEmail,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context,
            Action<Guid> GeneralFailHandler = null)
        {

            if (UserEmail.IsEmpty())
            {
                return AcctPswdResultCode.EmailEmpty;
            }

            UserEmail = UserEmail.Trim();

            var ID = UserReader.GetUserID(UserEmail);
            if (ID == null)
            {
                return AcctPswdResultCode.UserInvalid;
            }

            return await TryUpdatePasswordAsync(
                ID.Value,
                OldPassword,
                NewPassword,
                DeviceLogout,
                Context,
                GeneralFailHandler);
        }

        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public async Task<AcctPswdResultCode> TryUpdatePasswordAsync(
            long UserID,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context,
            Action<Guid> GeneralFailHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var taskGetRequestedUser = UserReader.GetUserAsync(UserID);

            //check for valid OLD password
            var pswdValidation = Shared.ValidateUserPswd(OldPassword);
            if (pswdValidation != 0)
            {
                return pswdValidation;
            }
            //check for valid NEW password
            pswdValidation = Shared.ValidateUserPswd(NewPassword);
            if (pswdValidation != 0)
            {
                return pswdValidation;
            }


            //check to see if the new password is the same as the old password
            if (OldPassword == NewPassword)
            {
                return AcctPswdResultCode.PswdNotChanged;
            }


            try
            {

                var modUser = await taskGetRequestedUser;
                if (modUser == null || modUser.ID == null)
                {
                    return AcctPswdResultCode.UserInvalid;
                }


                var authResult = await CoreFactory.Singleton.Auth.TryAuthenticateUserAsync(modUser.Email, OldPassword);
                if (authResult != 0)
                {
                    return AcctPswdResultCode.LoginFailed;
                }

                //try to change password
                var salt = SysSec.Membership.GeneratePassword(SALT_LEN, 0);
                string hashedPsd = null;
                try
                {
                    hashedPsd = NewPassword.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("C8CB51BB-E26F-46FF-A3F7-3762AD1708AC"));
                    return AcctPswdResultCode.UnknownError;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("E552CE2C-34C2-45EE-9D39-3E346A1FBFBF"));
                    return AcctPswdResultCode.UnknownError;
                }
                try
                {
                    await UpdateUserPassword_DBAsync(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("06C6E47C-69AD-4A3D-866C-DB797B645364"));
                    return AcctPswdResultCode.UnknownError;
                }



                //if everything worked, increment user version to force global re auth
                if (DeviceLogout)
                {
                    modUser.UpdateVersion();

                    //re auth current user to prevent lapse in service
                    await CoreFactory.Singleton.Auth.TrySetClientAuthTokenAsync(modUser.Email, NewPassword, false, Context);
                }


                //remove any recovery contexts
                modUser.GetRecoveryContext()?.DeleteAsync();


                return AcctPswdResultCode.Success;

            }
            catch (Exception ex)
            {
                var errCode = "17A42823-0767-4BA4-9F24-6A868509558B";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return AcctPswdResultCode.UnknownError;
            }
        }



        /// <summary>
        /// Attempt to recover account password using a recovery context ID and key
        /// </summary>
        /// <param name="RecoveryContextID"></param>
        /// <param name="RecoveryKey"></param>
        /// <param name="NewPassword"></param>
        /// <param name="DeviceLogout"></param>
        /// <param name="GeneralFailHandler"></param>
        /// <returns></returns>
        public async Task<AcctRecoveryResultCode> TryRecoverPasswordAsync(
            string RecoveryContextID,
            string RecoveryKey,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context,
            Action<Guid> GeneralFailHandler = null)
        {
            //TODO: fix side channel attack vector that would allow attacker to bypass attempt limit with large number of simultaneous requests
            //Possible solution:
            //Use string interning + thread lock to prevent concurrent threads using the same ContextID
            //Each call would get a shared string reference using string.intern
            //This shared reference can be used as a lock object

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!CoreFactory.Singleton.Properties.EnablePswdRecovery)
            {
                throw new InvalidOperationException("Password resets are not enabled");
            }




            var recoveryContext = await UserReader.GetRecoveryContextAsync(RecoveryContextID);

            if (recoveryContext == null)
            {
                return AcctRecoveryResultCode.RecoveryContextInvalid;
            }


            var resultCode = recoveryContext.ValidateRecoveryKey(RecoveryKey);
            if (resultCode != 0)
            {
                await recoveryContext.IncrementAttemptCountAsync();
                return resultCode;
            }


            var userID = recoveryContext.UserID;

            return await TryResetPasswordAsync(
                userID,
                NewPassword,
                DeviceLogout,
                Context,
                GeneralFailHandler);
        }





        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public async Task<AcctRecoveryResultCode> TryResetPasswordAsync(
            string UserEmail,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context,
            Action<Guid> GeneralFailHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (!CoreFactory.Singleton.Properties.EnablePswdRecovery)
            {
                throw new InvalidOperationException("Password resets are not enabled");
            }

            if (UserEmail.IsEmpty())
            {
                return AcctRecoveryResultCode.EmailEmpty;
            }

            UserEmail = UserEmail.Trim();

            var ID = await UserReader.GetUserIDAsync(UserEmail);

            if (ID == null)
            {
                return AcctRecoveryResultCode.UserInvalid;
            }

            return await TryResetPasswordAsync(
                ID.Value,
                NewPassword,
                DeviceLogout,
                Context,
                GeneralFailHandler);
        }

        /// <summary>
        /// Attempts to reset a user password.  System level function that overrides validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="NewPassword">New password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public async Task<AcctRecoveryResultCode> TryResetPasswordAsync(
            long UserID,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context,
            Action<Guid> GeneralFailHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!CoreFactory.Singleton.Properties.EnablePswdRecovery)
            {
                throw new InvalidOperationException("Password resets are not enabled");
            }


            var taskGetUser = UserReader.GetUserAsync(UserID);


            //check for valid password
            var pswdValidation = Shared.ValidateUserPswd(NewPassword);
            if (pswdValidation != 0)
            {
                return (AcctRecoveryResultCode)pswdValidation;
            }


            try
            {

                var modUser = await taskGetUser;
                if (modUser == null || modUser.ID == null)
                {
                    return AcctRecoveryResultCode.UserInvalid;
                }


                //try to change password
                var salt = SysSec.Membership.GeneratePassword(SALT_LEN, 0);
                string hashedPsd = null;
                try
                {
                    var hashType = CoreFactory.Singleton.Properties.PswdHashType;
                    hashedPsd = NewPassword.GetCryptoHash(hashType, salt);
                }
                catch (Exception ex1)
                {
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex1);
                    GeneralFailHandler?.Invoke(new Guid("87C7EC81-811A-480F-B132-7963FE0D657C"));

                    return AcctRecoveryResultCode.UnknownError;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("A3A419C1-60D1-46D4-A8E5-13B2C6A1AF20"));

                    return AcctRecoveryResultCode.UnknownError;
                }
                try
                {
                    await UpdateUserPassword_DBAsync(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("A36CA901-8A1F-4E73-ACE5-9DE68CF3B15A"));

                    return AcctRecoveryResultCode.UnknownError;
                }

                //if everything worked, increment user version to force global re auth
                modUser.UpdateVersion();

                //re auth current user to prevent lapse in service
                try
                {
                    await CoreFactory.Singleton.Auth.TrySetClientAuthTokenAsync(modUser.Email, NewPassword, false, Context);
                }
                catch
                {
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync("5F537D00-DB22-4C72-8D52-9FC557BAEED2");
                }

                //remove any recovery contexts
                modUser.GetRecoveryContext()?.Delete();



                return AcctRecoveryResultCode.Success;


            }
            catch (Exception ex2)
            {
                var errCode = "347A406B-85D2-4C6D-B966-3B3D81120DC3";
                Exception ex_outer = new Exception(errCode, ex2);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return AcctRecoveryResultCode.UnknownError;

            }
        }



        /// <summary>
        /// Delete user by ID.
        /// </summary>
        /// <param name="UserID"></param>
        public async Task DeleteUserAsync(long UserID)
        {
            var modUser = await UserReader.GetUserAsync(UserID);
            await _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user by Email
        /// </summary>
        /// <param name="Email"></param>
        public async Task DeleteUserAsync(string Email)
        {
            var modUser = await UserReader.GetUserAsync(Email);
            await _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user by Username and Domain
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        public async Task DeleteUserAsync(string Username, string Domain)
        {
            var modUser = UserReader.GetUser(Username, Domain);
            await _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="CmsUser"></param>
        private async Task _deleteUserAsync(User CmsUser)
        {
#pragma warning disable 612, 618
            try
            {
                if (CmsUser != null && CmsUser.ID != null)
                {
                    CmsUser.UpdateVersion();

                    await UserWriter.DeleteUserAsync((long)CmsUser.ID);
                }
            }
            catch (Exception ex)
            {
                var errCode = "7AA4C25A-458D-4184-A50A-C85779E5DDAF";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                throw new Exception();
            }
#pragma warning restore
        }

        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<(AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey)> TryCreateUserRecoveryContextAsync(
            string UserEmail,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null)
        {
            //check for valid email format
            if (!UserEmail.IsValidEmail())
            {
                return (AcctRecoveryResultCode.EmailInvalid, null, null);
            }


            long? id = await UserReader.GetUserIDAsync(UserEmail);

            if (id == null)
            {
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }


            return await TryCreateUserRecoveryContextAsync(
                id.Value,
                IsOptional,
                GeneralFailHandler);
        }

        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<(AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey)> TryCreateUserRecoveryContextAsync(
            long UserID,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null)
        {
            try
            {
                var cmsUser = await UserReader.GetUserAsync(UserID);

                if (cmsUser == null || cmsUser.ID == null)
                {
                    return (AcctRecoveryResultCode.UserInvalid, null, null);
                }


                //prevent a new recovery context from being made if the user already has an active forced reset
                //a new recovery context would override the prior and allow a bypass
                var recoveryContext = cmsUser.GetRecoveryContext();
                if (recoveryContext != null && !recoveryContext.IsOptional)
                {
                    return (AcctRecoveryResultCode.UserInvalid, null, null);
                }


                var hashType = CoreFactory.Singleton.Properties.PswdHashType;
                string recoveryKey = SysSec.Membership.GeneratePassword(R_KEY_LEN, 5);
                string hashedKey = recoveryKey.GetCryptoHash(hashType);

#pragma warning disable 612, 618
                var context = await UserWriter.CreateRecoveryContextAsync(UserID, hashedKey, true, true);
#pragma warning restore


                if (context == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("17849D2D-07D7-4BC4-939A-51CD1D4D0707"));
                    return (AcctRecoveryResultCode.UnknownError, null, null);
                }


                return (AcctRecoveryResultCode.Success, context, recoveryKey);
            }
            catch
            {
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }

        }


        private static async Task UpdateUserPassword_DBAsync(string UserEmail, string Password, string Salt)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!UserReader.DoesUserExist(UserEmail))
            {
                throw new InvalidOperationException("User does not exist");
            }

            await SqlWorker.ExecNonQueryAsync(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdatePasswordByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = UserEmail;
                    cmd.Parameters.Add("@PswdHash", SqlDbType.NVarChar).Value = Password;
                    cmd.Parameters.Add("@Salt", SqlDbType.NVarChar).Value = Salt;
                });
        }


        private static async Task UpdateUserPassword_DBAsync(long UserID, string Password, string Salt)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!UserReader.DoesUserExist(UserID))
            {
                throw new InvalidOperationException("User does not exist");
            }

            await SqlWorker.ExecNonQueryAsync(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdatePasswordByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@PswdHash", SqlDbType.NVarChar).Value = Password;
                    cmd.Parameters.Add("@Salt", SqlDbType.NVarChar).Value = Salt;
                });
        }
    }
}
