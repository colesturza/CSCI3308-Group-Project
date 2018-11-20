using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SysSec = System.Web.Security;
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
    public sealed partial class AccountManager : IAccountManager
    {
        private const short EMAIL_MIN_LEN = 3;
        private const short EMAIL_MAX_LEN = 250;
        private const short SALT_LEN = 50;
        private const short USER_VERSION_LEN = 10;
        private const short R_KEY_LEN = 20;


        /// <summary>
        /// Try to create a new user in the CMS system
        /// </summary>
        /// <param name="UserEmail">New user email</param>
        /// <param name="UserPassword">New user password</param>
        /// <param name="AttemptAutoLogin">Should system automatically login user after creating account</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler">Args: new user object, auto login [T|F]</param>
        /// <returns>Status Flag</returns>
        public AcctCreateResultCode TryCreateUser(
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


            //TODO: finalize trims
            //TODO: validate year against list
            Shared.TryCreate_HandleAttrTrim(ref NewUser);


            var attrValidation = Shared.TryCreate_ValidateUserAttributes(NewUser);
            if (attrValidation != AcctCreateResultCode.Success)
            {
                return attrValidation;
            }

            var pswdValidation = Shared.ValidateUserPswd(NewUser);
            if (pswdValidation != (int)AcctResultCode.Success)
            {
                return (AcctCreateResultCode)pswdValidation;
            }


            //check for duplicate email
            if (UserReader.DoesUserExist(NewUser.Email))
            {
                return AcctCreateResultCode.EmailDuplicate;
            }

            //Validate user domain and school
            var domain = NewUser.Email.GetEmailDomain();

            var tmpSchool = SchoolReader.GetSchoolByDomain(domain);
            if (tmpSchool == null || tmpSchool.ID == null)
            {
                return AcctCreateResultCode.EmailDomainInvalid;
            }
            NewUser.SchoolID = tmpSchool.ID;


            //check for duplicate username
            if (UserReader.DoesUserExist(NewUser.Username, domain))
            {
                return AcctCreateResultCode.UsernameDuplicate;
            }


            //check for valid major (chosen via dropdown)
            var major = NewUser.Major;
            var majorValidationSet = SchoolMajorReader
                                            .GetMajorsBySchool(NewUser.SchoolID.Value)
                                            .Select(x => x.Name)
                                            .ToHashSet();

            if (!majorValidationSet.Contains(major))
            {
                return AcctCreateResultCode.MajorInvalid;
            }

            Shared.TryCreate_HandleUserDefaults(ref NewUser);


            try
            {
                //create CMS user
                var userID = UserWriter.TryCreateUser(NewUser);


                if (userID == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("CE1989AB-3C46-4810-B4F8-432D752C85A1"));
                    return AcctCreateResultCode.UnknownError;
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
                    UserWriter.TryPurgeUser((long)userID);
                    throw new Exception(ResponseStrings.AccountError.ACCOUNT_FAIL);
                }
                if (pswdHash.IsEmpty())
                {
                    UserWriter.TryPurgeUser((long)userID);
                    throw new Exception(ResponseStrings.AccountError.ACCOUNT_FAIL);
                }
                try
                {
                    //SET DB PASSWORD
                    UpdateUserPassword_DB((long)userID, pswdHash, salt);
                }
                catch
                {
                    UserWriter.TryPurgeUser((long)userID);
                    throw new Exception(ResponseStrings.AccountError.ACCOUNT_FAIL);
                }




                //get cms user
                var cmsUser = UserReader.GetUser(userID.Value);
                var confirmToken = UserReader.GetConfirmToken(userID.Value);


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
                        return AcctCreateResultCode.Success;
                    }
                }
                else if (!CoreFactory.Singleton.Properties.AutoConfirmNewAccounts)
                {
                    var siteName = CoreFactory.Singleton.Properties.SiteFriendlyName;

                    var msg = new SmtpMessage_ConfirmAcct($"{siteName} Account Confirmation", siteName, NewUser.Email)
                    {
                        ConfirmationURL = confirmToken.GetURL()
                    };

                    var emailSendStatus = CoreFactory.Singleton.Mail.TrySendMessage(msg);
                    if (emailSendStatus != SmtpResultCode.Success)
                    {
                        var errCode = "AEBDE62B-31D5-4B48-8D26-3123AA5219A3";
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

                var errCode = "A983AFB8-920A-4850-9197-3DDE7F6E89CC";
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
        public bool TryConfirmUser(string RefUID) => TryConfirmUser(RefUID, out _);

        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        public bool TryConfirmUser(string RefUID, out string Status)
        {
            try
            {
                return ConfirmUser(RefUID, out Status);
            }
            catch (Exception ex)
            {
                Status = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        public bool ConfirmUser(string RefUID, out string Status)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (RefUID.IsEmpty())
            {
                Status = $"Invalid {nameof(RefUID)} format";
                return false;
            }

            if (!RefUID.RgxIsMatch(RgxPtrn.User.REF_UID_B))
            {
                Status = $"Invalid {nameof(RefUID)} format";
                return false;
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


            var isValid = UserWriter.ConfirmUser(RefUID, minCreatedDate);

            if (isValid)
            {
                Status = "Success";
                return true;
            }
            else
            {
                Status = "Confirmation Token Not Valid";
                return false;
            }
        }


        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        public bool TryUpdateApprovalStatus(long UserID, bool IsApproved)
        {
            try
            {
                UserWriter.UpdateUserApproval(UserID, IsApproved);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        public void UpdateUserApprovalStatus(long UserID, bool IsApproved)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            UserWriter.UpdateUserApproval(UserID, IsApproved);
        }


        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public AcctPswdResultCode TryUpdatePassword(
            string UserEmail,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
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

            return TryUpdatePassword(
                ID.Value,
                OldPassword,
                NewPassword,
                DeviceLogout,
                GeneralFailHandler);
        }

        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public AcctPswdResultCode TryUpdatePassword(
            long UserID,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            Action<Guid> GeneralFailHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            //check for valid OLD password
            var pswdValidation = Shared.ValidateUserPswd(OldPassword);
            if (pswdValidation != (int)AcctResultCode.Success)
            {
                return pswdValidation;
            }
            //check for valid NEW password
            pswdValidation = Shared.ValidateUserPswd(NewPassword);
            if (pswdValidation != (int)AcctResultCode.Success)
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
                if (!UserReader.DoesUserExist(UserID))
                {
                    return AcctPswdResultCode.UserInvalid;
                }

                var modUser = UserReader.GetUser(UserID);
                if (modUser == null || modUser.ID == null)
                {
                    return AcctPswdResultCode.UserInvalid;
                }


                var authStatusCode = CoreFactory.Singleton.Auth.TryAuthenticateUser(modUser.Email, OldPassword);
                if (authStatusCode != AuthResultCode.Success)
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
                    GeneralFailHandler?.Invoke(new Guid("B6877027-52A2-41D4-949F-E47578305C44"));
                    return AcctPswdResultCode.UnknownError;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("F70C21AA-2469-477A-9518-7CBFA7BC6F88"));
                    return AcctPswdResultCode.UnknownError;
                }
                try
                {
                    UpdateUserPassword_DB(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("6D23ECC3-1D36-4F81-8EE6-9F334E97265F"));
                    return AcctPswdResultCode.UnknownError;
                }



                //if everything worked, increment user version to force global re auth
                if (DeviceLogout)
                {
                    modUser.UpdateVersion();

                    //re auth current user to prevent lapse in service
                    CoreFactory.Singleton.Auth.TrySetClientAuthToken(modUser.Email, NewPassword, false);
                }


                //remove any recovery contexts
                modUser.GetRecoveryContext()?.Delete();

                return AcctPswdResultCode.Success;

            }
            catch (Exception ex)
            {
                var errCode = "B9932471-7779-4710-A97E-BB1FA147A995";
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
        public AcctRecoveryResultCode TryRecoverPassword(
            string RecoveryContextID,
            string RecoveryKey,
            string NewPassword,
            bool DeviceLogout,
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




            var recoveryContext = UserReader.GetRecoveryContext(RecoveryContextID);

            if (recoveryContext == null)
            {
                return AcctRecoveryResultCode.RecoveryContextInvalid;
            }


            var resultCode = recoveryContext.ValidateRecoveryKey(RecoveryKey);


            if (resultCode != AcctRecoveryResultCode.Success)
            {
                recoveryContext.IncrementAttemptCount();
                return resultCode;
            }


            var userID = recoveryContext.UserID;

            return TryResetPassword(
                userID,
                NewPassword,
                DeviceLogout,
                GeneralFailHandler);
        }




        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public AcctRecoveryResultCode TryResetPassword(
            string UserEmail,
            string NewPassword,
            bool DeviceLogout,
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

            var ID = UserReader.GetUserID(UserEmail);
            if (ID == null)
            {
                return AcctRecoveryResultCode.UserInvalid;
            }

            return TryResetPassword(
                ID.Value,
                NewPassword,
                DeviceLogout,
                GeneralFailHandler);
        }

        /// <summary>
        /// Attempts to reset a user password.  System level function that overrides validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="NewPassword">New password</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public AcctRecoveryResultCode TryResetPassword(
            long UserID,
            string NewPassword,
            bool DeviceLogout,
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

            //check for valid password
            var pswdValidation = Shared.ValidateUserPswd(NewPassword);
            if (pswdValidation != (int)AcctResultCode.Success)
            {
                return (AcctRecoveryResultCode)pswdValidation;
            }


            try
            {
                if (!UserReader.DoesUserExist(UserID))
                {
                    return AcctRecoveryResultCode.UserInvalid;
                }

                var modUser = UserReader.GetUser(UserID);
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
                    GeneralFailHandler?.Invoke(new Guid("AA3E2DB3-5CCF-400D-8046-1D982E723F58"));

                    return AcctRecoveryResultCode.UnknownError;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("798206EE-253A-41F8-BF1F-D5FAC1608D54"));
                    return AcctRecoveryResultCode.UnknownError;
                }
                try
                {
                    UpdateUserPassword_DB(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("7A6840DA-B08B-4972-B85F-11B45B45E3B0"));
                    return AcctRecoveryResultCode.UnknownError;
                }

                //if everything worked, increment user version to force global re auth
                if (DeviceLogout)
                {
                    modUser.UpdateVersion();

                    //re auth current user to prevent lapse in service
                    CoreFactory.Singleton.Auth.TrySetClientAuthToken(modUser.Email, NewPassword, false);
                }



                //re auth current user to prevent lapse in service
                try
                {
                    CoreFactory.Singleton.Auth.TrySetClientAuthToken(modUser.Email, NewPassword, false);
                }
                catch
                {
                    CoreFactory.Singleton.Logging.CreateErrorLogAsync("6B11B5D6-6B8D-419C-B45D-0444EE23EA32");
                }

                //remove any recovery contexts
                modUser.GetRecoveryContext()?.Delete();


                return AcctRecoveryResultCode.Success;


            }
            catch (Exception ex2)
            {
                var errCode = "8641F1E3-B29B-4CC1-ABA5-90B8693625EE";
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
        public void DeleteUser(long UserID)
        {
            var modUser = UserReader.GetUser(UserID);
            _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user by Email
        /// </summary>
        /// <param name="Email"></param>
        public void DeleteUser(string Email)
        {
            var modUser = UserReader.GetUser(Email);
            _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user by Username and Domain
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        public void DeleteUser(string Username, string Domain)
        {
            var modUser = UserReader.GetUser(Username, Domain);
            _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="CmsUser"></param>
        private void _deleteUser(User CmsUser)
        {
            try
            {
                if (CmsUser != null && CmsUser.ID != null)
                {
                    CmsUser.UpdateVersion();

                    UserWriter.DeleteUser((long)CmsUser.ID);
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
        public (AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey) TryCreateUserRecoveryContext(
            string UserEmail,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null)
        {
            //check for valid email format
            if (!UserEmail.IsValidEmail())
            {
                return (AcctRecoveryResultCode.EmailInvalid, null, null);
            }


            long? id = UserReader.GetUserID(UserEmail);

            if (id == null)
            {
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }


            return TryCreateUserRecoveryContext(
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
        public (AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey) TryCreateUserRecoveryContext(
            long UserID,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null)
        {
            try
            {
                var cmsUser = UserReader.GetUser(UserID);

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

                string recoveryKey = SysSec.Membership.GeneratePassword(R_KEY_LEN, 5);
                string hashedKey = recoveryKey.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType);


                var context = UserWriter.CreateRecoveryContext(UserID, hashedKey, true, true);

                if (context == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("B2AA0C33-A7A5-4026-ADC1-687C8406E8F8"));
                    return (AcctRecoveryResultCode.UnknownError, null, null);
                }



                return (AcctRecoveryResultCode.Success, context, recoveryKey);
            }
            catch
            {
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }

        }


        private static void UpdateUserPassword_DB(string UserEmail, string Password, string Salt)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!UserReader.DoesUserExist(UserEmail))
            {
                throw new InvalidOperationException("User does not exist");
            }

            SqlWorker.ExecNonQuery(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdatePasswordByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = UserEmail;
                    cmd.Parameters.Add("@PswdHash", SqlDbType.NVarChar).Value = Password;
                    cmd.Parameters.Add("@Salt", SqlDbType.NVarChar).Value = Salt;
                });
        }


        private static void UpdateUserPassword_DB(long UserID, string Password, string Salt)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!UserReader.DoesUserExist(UserID))
            {
                throw new InvalidOperationException("User does not exist");
            }

            SqlWorker.ExecNonQuery(
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
