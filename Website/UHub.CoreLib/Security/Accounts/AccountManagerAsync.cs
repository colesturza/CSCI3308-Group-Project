using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SysSec = System.Web.Security;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Management;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.SmtpInterop;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.SchoolMajors.Management;
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Security.Accounts.Interfaces;
using UHub.CoreLib.Security.Authentication;
using System.Web;

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
        public async Task<AccountResultCode> TryCreateUserAsync(
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
                return AccountResultCode.UserInvalid;
            }

            var taskDoesEmailExist = UserReader.DoesUserExistAsync(NewUser.Email);
            var taskSchoolMajors = SchoolMajorReader.GetMajorsBySchoolAsync(NewUser.SchoolID.Value);



            //TODO: finalize trims
            //TODO: validate year against list
            NewUser.Year = NewUser.Year?.Trim();
            NewUser.Company = NewUser.Company?.Trim();
            NewUser.Email = NewUser.Email?.Trim();


            //ensure email is populated
            if (NewUser.Email.IsEmpty())
            {
                return AccountResultCode.EmailEmpty;
            }
            //check for valid email length
            if (NewUser.Email.Length < EMAIL_MIN_LEN || NewUser.Email.Length > EMAIL_MAX_LEN)
            {
                return AccountResultCode.EmailInvalid;
            }
            //check for valid email format
            if (!NewUser.Email.IsValidEmail())
            {
                return AccountResultCode.EmailInvalid;
            }


            //check for valid username
            if (NewUser.Username.RgxIsMatch(RgxPatterns.User.USERNAME_B))
            {
                return AccountResultCode.UsernameInvalid;
            }


            //check for invalid user name
            if(NewUser.Name.RgxIsMatch(RgxPatterns.User.NAME_B))
            {
                return AccountResultCode.NameInvalid;
            }

            //TODO: finalize attr validation



            //Validate user domain and school
            var domain = NewUser.Email.GetEmailDomain();
            var taskGetUserSchool = SchoolReader.GetSchoolByDomainAsync(domain);
            var taskDoesUsernameExist = UserReader.DoesUserExistAsync(NewUser.Username, domain);


            //ensure pswd is populated
            if (NewUser.Password.IsEmpty())
            {
                return AccountResultCode.PswdEmpty;
            }
            //check for valid password
            if (!Regex.IsMatch(NewUser.Password, CoreFactory.Singleton.Properties.PswdStrengthRegex))
            {
                return AccountResultCode.PswdInvalid;
            }

            //check for duplicate email
            if (await taskDoesEmailExist)
            {
                return AccountResultCode.EmailDuplicate;
            }


            var tmpSchool = await taskGetUserSchool;
            if (tmpSchool == null || tmpSchool.ID == null)
            {
                return AccountResultCode.EmailDomainInvalid;
            }
            NewUser.SchoolID = tmpSchool.ID;


            //check for duplicate username
            if (await taskDoesUsernameExist)
            {
                return AccountResultCode.UsernameDuplicate;
            }


            //check for valid major (chosen via dropdown)
            var major = NewUser.Major;
            var majorValidationSet = (await taskSchoolMajors)
                                        .Select(x => x.Name)
                                        .ToHashSet();


            if (!majorValidationSet.Contains(major))
            {
                return AccountResultCode.MajorInvalid;
            }


            //set property constants
            bool isConfirmed = CoreFactory.Singleton.Properties.AutoConfirmNewAccounts;
            bool isApproved = CoreFactory.Singleton.Properties.AutoApproveNewAccounts;
            string userVersion = SysSec.Membership.GeneratePassword(USER_VERSION_LEN, 0);
            //sterilize for token processing
            userVersion = userVersion.Replace('|', '0');


            NewUser.IsConfirmed = isConfirmed;
            NewUser.IsApproved = isApproved;
            NewUser.Version = userVersion;

            try
            {
                //create CMS user
                var userID = await UserWriter.TryCreateUserAsync(NewUser);


                if (userID == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("CE1989AB-3C46-4810-B4F8-432D752C85A1"));
                    return AccountResultCode.UnknownError;
                }


                //try to create password
                //if failed, then purge the remaining CMS account components so user can try again


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
                        var errCode = "A275649B-AD89-43E3-8DE2-B81B6F47FE6A";
                        CoreFactory.Singleton.Logging.CreateErrorLogAsync(errCode);


                        SuccessHandler?.Invoke(cmsUser, false);
                        return AccountResultCode.Success;
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

                    if (!CoreFactory.Singleton.Mail.TrySendMessage(msg))
                    {

                        var errCode = "AEBDE62B-31D5-4B48-8D26-3123AA5219A3";
                        CoreFactory.Singleton.Logging.CreateErrorLogAsync(errCode);
                        GeneralFailHandler?.Invoke(new Guid(errCode));

                        return AccountResultCode.UnknownError;
                    }
                }


                SuccessHandler?.Invoke(cmsUser, canLogin);
                return AccountResultCode.Success;
            }
            catch (DuplicateNameException)
            {
                return AccountResultCode.EmailDuplicate;
            }
            catch (Exception ex)
            {
                var errCode = "A983AFB8-920A-4850-9197-3DDE7F6E89CC";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return AccountResultCode.UnknownError;
            }
        }



        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        public async Task<(bool StatusFlag, string StatusMsg)> ConfirmUserAsync(string RefUID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (RefUID.IsEmpty())
            {
                return (false, $"Invalid {nameof(RefUID)} format");
            }

            if (!RefUID.RgxIsMatch(RgxPatterns.User.REF_UID_B))
            {
                return (false, $"Invalid {nameof(RefUID)} format");
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
                var isValid = await UserWriter.ConfirmUserAsync(RefUID, minCreatedDate);

                if (isValid)
                {
                    return (true, "Success");
                }
                else
                {
                    return (false, "Confirmation Token Not Valid");
                }
            }
            catch
            {
                return (false, "Uknown Error");
            }
        }


        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        public async Task<bool> TryUpdateUserApprovalStatusAsync(long UserID, bool IsApproved)
        {
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
        public async Task<AccountResultCode> TryUpdatePasswordAsync(
            string UserEmail,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context,
            Action<Guid> GeneralFailHandler = null)
        {

            if (UserEmail.IsEmpty())
            {
                return AccountResultCode.EmailEmpty;
            }

            UserEmail = UserEmail.Trim();

            var ID = UserReader.GetUserID(UserEmail);
            if (ID == null)
            {
                return AccountResultCode.UserInvalid;
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
        public async Task<AccountResultCode> TryUpdatePasswordAsync(
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
            var pswdStrength = CoreFactory.Singleton.Properties.PswdStrengthRegex;

            if (OldPassword.IsEmpty())
            {
                return AccountResultCode.PswdEmpty;
            }

            if (NewPassword.IsEmpty())
            {
                return AccountResultCode.PswdEmpty;
            }


            if (!Regex.IsMatch(OldPassword, pswdStrength))
            {
                return AccountResultCode.PswdInvalid;
            }

            //check for valid NEW password
            if (!Regex.IsMatch(NewPassword, pswdStrength))
            {
                return AccountResultCode.PswdInvalid;
            }

            //check to see if the new password is the same as the old password
            if (OldPassword == NewPassword)
            {
                return AccountResultCode.PswdNotChanged;
            }


            try
            {

                var modUser = await taskGetRequestedUser;
                if (modUser == null || modUser.ID == null)
                {
                    return AccountResultCode.UserInvalid;
                }


                var authResult = await CoreFactory.Singleton.Auth.TryAuthenticateUserAsync(modUser.Email, OldPassword);
                var isAuthValid = (authResult == AuthResultCode.Success);

                if (!isAuthValid)
                {
                    return AccountResultCode.LoginFailed;
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
                    GeneralFailHandler?.Invoke(new Guid("B6877027-52A2-41D4-949F-E47578305C44"));
                    return AccountResultCode.UnknownError;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("F70C21AA-2469-477A-9518-7CBFA7BC6F88"));
                    return AccountResultCode.UnknownError;
                }
                try
                {
                    UpdateUserPassword_DB(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("6D23ECC3-1D36-4F81-8EE6-9F334E97265F"));
                    return AccountResultCode.UnknownError;
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


                return AccountResultCode.Success;

            }
            catch (Exception ex)
            {
                var errCode = "B9932471-7779-4710-A97E-BB1FA147A995";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return AccountResultCode.UnknownError;
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
        public async Task<AccountResultCode> TryRecoverPasswordAsync(
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
                return AccountResultCode.RecoveryContextInvalid;
            }


            var resultCode = recoveryContext.ValidateRecoveryKey(RecoveryKey);


            if (resultCode != AccountResultCode.Success)
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
        public async Task<AccountResultCode> TryResetPasswordAsync(
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
                return AccountResultCode.EmailEmpty;
            }

            UserEmail = UserEmail.Trim();

            var ID = await UserReader.GetUserIDAsync(UserEmail);

            if (ID == null)
            {
                return AccountResultCode.UserInvalid;
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
        public async Task<AccountResultCode> TryResetPasswordAsync(
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
            if (!Regex.IsMatch(NewPassword, CoreFactory.Singleton.Properties.PswdStrengthRegex))
            {
                return AccountResultCode.PswdInvalid;
            }


            try
            {

                var modUser = await taskGetUser;
                if (modUser == null || modUser.ID == null)
                {
                    return AccountResultCode.UserInvalid;
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
                    GeneralFailHandler?.Invoke(new Guid("AA3E2DB3-5CCF-400D-8046-1D982E723F58"));

                    return AccountResultCode.UnknownError;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("798206EE-253A-41F8-BF1F-D5FAC1608D54"));

                    return AccountResultCode.UnknownError;
                }
                try
                {
                    UpdateUserPassword_DB(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("7A6840DA-B08B-4972-B85F-11B45B45E3B0"));

                    return AccountResultCode.UnknownError;
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
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync("6B11B5D6-6B8D-419C-B45D-0444EE23EA32");
                }

                //remove any recovery contexts
                modUser.GetRecoveryContext()?.Delete();


                
                return AccountResultCode.Success;


            }
            catch (Exception ex2)
            {
                var errCode = "8641F1E3-B29B-4CC1-ABA5-90B8693625EE";
                Exception ex_outer = new Exception(errCode, ex2);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return AccountResultCode.UnknownError;

            }
        }



        /// <summary>
        /// Delete user by ID.
        /// </summary>
        /// <param name="UserID"></param>
        public async Task DeleteUserAsync(long UserID)
        {
            var modUser = await UserReader.GetUserAsync(UserID);
            _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user by Email
        /// </summary>
        /// <param name="Email"></param>
        public async Task DeleteUserAsync(string Email)
        {
            var modUser = await UserReader.GetUserAsync(Email);
            _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user by Username and Domain
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        public async Task DeleteUserAsync(string Username, string Domain)
        {
            var modUser = UserReader.GetUser(Username, Domain);
            _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="CmsUser"></param>
        private async Task _deleteUserAsync(User CmsUser)
        {
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
                var errCode = "54879964-C1BD-420C-B54D-BFBECFB71A52";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);
                throw new Exception();
            }
        }

        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<(AccountResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey)> TryCreateUserRecoveryContextAsync(
            string UserEmail,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null)
        {
            //check for valid email format
            if (!UserEmail.IsValidEmail())
            {
                return (AccountResultCode.EmailInvalid, null, null);
            }


            long? id = await UserReader.GetUserIDAsync(UserEmail);

            if (id == null)
            {
                return (AccountResultCode.UserInvalid, null, null);
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
        public async Task<(AccountResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey)> TryCreateUserRecoveryContextAsync(
            long UserID,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null)
        {
            try
            {
                var cmsUser = await UserReader.GetUserAsync(UserID);

                if (cmsUser == null || cmsUser.ID == null)
                {
                    return (AccountResultCode.UserInvalid, null, null);
                }


                //prevent a new recovery context from being made if the user already has an active forced reset
                //a new recovery context would override the prior and allow a bypass
                var recoveryContext = cmsUser.GetRecoveryContext();
                if (recoveryContext != null && !recoveryContext.IsOptional)
                {
                    return (AccountResultCode.UserInvalid, null, null);
                }


                var hashType = CoreFactory.Singleton.Properties.PswdHashType;
                string recoveryKey = SysSec.Membership.GeneratePassword(R_KEY_LEN, 5);
                string hashedKey = recoveryKey.GetCryptoHash(hashType);


                var context = await UserWriter.CreateRecoveryContextAsync(UserID, hashedKey, true, true);

                if (context == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("B2AA0C33-A7A5-4026-ADC1-687C8406E8F8"));
                    return (AccountResultCode.UnknownError, null, null);
                }


                return (AccountResultCode.Success, context, recoveryKey);
            }
            catch
            {
                return (AccountResultCode.UserInvalid, null, null);
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
