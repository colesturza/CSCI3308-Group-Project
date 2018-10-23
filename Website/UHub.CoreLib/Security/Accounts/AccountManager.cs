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

namespace UHub.CoreLib.Security.Accounts
{
    /// <summary>
    /// Wrapper for UserWriter functionality.  Controls user account create/edit/delete functionality while also adding error callback functionality
    /// </summary>
    public static class AccountManager
    {
        private const short minEmailLen = 3;
        private const short maxEmailLen = 250;
        private const short SALT_LENGTH = 50;
        private const short VERSION_LENGTH = 20;
        private const short R_KEY_LENGTH = 20;


        /// <summary>
        /// Try to create a new user in the CMS system
        /// </summary>
        /// <param name="UserEmail">New user email</param>
        /// <param name="UserPassword">New user password</param>
        /// <param name="AttemptAutoLogin">Should system automatically login user after creating account</param>
        /// <param name="InvalidEmailHandler">Error handler in case user email is invalid</param>
        /// <param name="DuplicateEmailHandler">Error handler in case user email already exists in CMS</param>
        /// <param name="InvalidPasswordHandler">Error handler in case password is invalid</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler">Args: new user object, auto login [T|F]</param>
        /// <returns></returns>
        public static bool TryCreateUser(User NewUser, bool AttemptAutoLogin,
            Action<AccountResultCode> ArgFailHandler = null,
            Action<Guid> GeneralFailHandler = null,
            Action<User, bool> SuccessHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            NewUser.Email = NewUser.Email?.Trim();

            //ensure email is populated
            if (NewUser.Email.IsEmpty())
            {
                ArgFailHandler?.Invoke(AccountResultCode.EmailEmpty);
                return false;
            }
            //check for valid email length
            if (NewUser.Email.Length < minEmailLen || NewUser.Email.Length > maxEmailLen)
            {
                ArgFailHandler?.Invoke(AccountResultCode.EmailInvalid);
                return false;
            }
            //check for valid email format
            if (!NewUser.Email.IsValidEmail())
            {
                ArgFailHandler?.Invoke(AccountResultCode.EmailInvalid);
                return false;
            }

            //ensure pswd is populated
            if (NewUser.Password.IsEmpty())
            {
                ArgFailHandler?.Invoke(AccountResultCode.PswdEmpty);
                return false;
            }
            //check for valid password
            if (!Regex.IsMatch(NewUser.Password, CoreFactory.Singleton.Properties.PswdStrengthRegex))
            {
                ArgFailHandler?.Invoke(AccountResultCode.PswdInvalid);
                return false;
            }

            //check for duplicate email
            if (UserReader.DoesUserExist(NewUser.Email, UserRefType.Email))
            {
                ArgFailHandler?.Invoke(AccountResultCode.EmailDuplicate);
                return false;
            }

            //check for duplicate username
            if (UserReader.DoesUserExist(NewUser.Username, UserRefType.Username))
            {
                ArgFailHandler?.Invoke(AccountResultCode.UsernameDuplicate);
                return false;
            }


            //Validate user domain and school
            var domain = NewUser.Email.Substring(NewUser.Email.IndexOf("@"));

            var tmpSchool = SchoolReader.GetSchoolByDomain(domain);
            if (tmpSchool == null || tmpSchool.ID == null)
            {
                ArgFailHandler?.Invoke(AccountResultCode.EmailDomainInvalid);
                return false;
            }
            NewUser.SchoolID = tmpSchool.ID;


            //check for valid major (chosen via dropdown)
            var major = NewUser.Major;
            var majorValidationSet = SchoolMajorReader
                                            .GetMajorsBySchool(NewUser.SchoolID.Value)
                                            .Select(x => x.Name)
                                            .ToHashSet();

            if (!majorValidationSet.Contains(major))
            {
                ArgFailHandler?.Invoke(AccountResultCode.MajorInvalid);
                return false;
            }



            //set property constants
            bool isConfirmed = CoreFactory.Singleton.Properties.AutoConfirmNewAccounts;
            bool isApproved = CoreFactory.Singleton.Properties.AutoApproveNewAccounts;
            string version = SysSec.Membership.GeneratePassword(VERSION_LENGTH, 0);

            NewUser.IsConfirmed = isConfirmed;
            NewUser.IsApproved = isApproved;
            NewUser.Version = version;

            try
            {
                //create CMS user
                var userID = UserWriter.TryCreateUser(NewUser);


                if (userID == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("CE1989AB-3C46-4810-B4F8-432D752C85A1"));
                    return false;
                }


                //try to create password
                //if failed, then purge the remaining CMS account components so user can try again


                var salt = SysSec.Membership.GeneratePassword(SALT_LENGTH, 0);
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


                //attempt autologin
                //autoconfirm user -> auto login
                bool canLogin = AttemptAutoLogin && CoreFactory.Singleton.Properties.AutoConfirmNewAccounts && CoreFactory.Singleton.Properties.AutoApproveNewAccounts;


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
                        var errCode = "A275649B-AD89-43E3-8DE2-B81B6F47FE6A";
                        CoreFactory.Singleton.Logging.CreateErrorLog(errCode);

                        SuccessHandler?.Invoke(cmsUser, false);
                        return true;
                    }
                }
                else if (!CoreFactory.Singleton.Properties.AutoConfirmNewAccounts)
                {
                    var siteName = CoreFactory.Singleton.Properties.SiteFriendlyName;

                    var msg = new SmtpMessage_ConfirmAcct($"{siteName} Account Confirmation", siteName, NewUser.Email)
                    {
                        ConfirmationURL = cmsUser.GetConfirmationURL()
                    };

                    if (!SmtpManager.TrySendMessage(msg))
                    {
                        var errCode = "AEBDE62B-31D5-4B48-8D26-3123AA5219A3";
                        CoreFactory.Singleton.Logging.CreateErrorLog(errCode);
                        GeneralFailHandler?.Invoke(new Guid(errCode));

                        return false;
                    }
                }



                SuccessHandler?.Invoke(cmsUser, canLogin);
                return true;
            }
            catch (DuplicateNameException)
            {
                ArgFailHandler?.Invoke(AccountResultCode.EmailDuplicate);
                return false;
            }
            catch (Exception ex)
            {
                var errCode = "A983AFB8-920A-4850-9197-3DDE7F6E89CC";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return false;
            }
        }

        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ConfirmUser(string RefUID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (RefUID.IsEmpty())
            {
                throw new ArgumentException($"Invalid {nameof(RefUID)} format");
            }

            if (!RefUID.RgxIsMatch($"^{RgxPatterns.User.REF_UID}$"))
            {
                throw new ArgumentException($"Invalid {nameof(RefUID)} format");
            }

            UserWriter.ConfirmUser(RefUID);
        }

        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="IsApproved">Approval Status</param>
        public static void UpdateUserApprovalStatus(long UserID, bool IsApproved)
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
        /// <param name="InvalidUserHandler">Error handler in case user does not exist</param>
        /// <param name="InvalidPasswordHandler">Error handler in case new password is invalid</param>
        /// <param name="PasswordNotChangedHandler">Error handler in case the new password is the same as the old password</param>
        /// <param name="LoginFailHandler">Error handler in case the user cannot be authenticated with the current password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler"></param>
        /// <returns>Status flag</returns>
        public static bool TryUpdatePassword(
            string UserEmail,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout = true,
            Action<AccountResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null)
        {

            if (UserEmail.IsEmpty())
            {
                ResultHandler?.Invoke(AccountResultCode.EmailEmpty);
                return false;
            }

            UserEmail = UserEmail.Trim();

            var ID = UserReader.GetUserID(UserEmail, UserRefType.Email);
            if (ID == null)
            {
                ResultHandler?.Invoke(AccountResultCode.UserInvalid);
                return false;
            }

            return TryUpdatePassword(
                ID.Value,
                OldPassword,
                NewPassword,
                DeviceLogout,
                ResultHandler,
                GeneralFailHandler);
        }

        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="InvalidUserHandler">Error handler in case user does not exist</param>
        /// <param name="InvalidPasswordHandler">Error handler in case new password is invalid</param>
        /// <param name="PasswordNotChangedHandler">Error handler in case the new password is the same as the old password</param>
        /// <param name="LoginFailHandler">Error handler in case the user cannot be authenticated with the current password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler"></param>
        /// <returns>Status flag</returns>
        public static bool TryUpdatePassword(
            long UserID,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout = true,
            Action<AccountResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            //check for valid OLD password
            var pswdStrength = CoreFactory.Singleton.Properties.PswdStrengthRegex;

            if (OldPassword.IsEmpty())
            {
                ResultHandler?.Invoke(AccountResultCode.PswdEmpty);
                return false;
            }

            if (NewPassword.IsEmpty())
            {
                ResultHandler?.Invoke(AccountResultCode.PswdEmpty);
                return false;
            }


            if (!Regex.IsMatch(OldPassword, pswdStrength))
            {
                ResultHandler?.Invoke(AccountResultCode.PswdInvalid);
                return false;
            }

            //check for valid NEW password
            if (!Regex.IsMatch(NewPassword, pswdStrength))
            {
                ResultHandler?.Invoke(AccountResultCode.PswdInvalid);
                return false;
            }

            //check to see if the new password is the same as the old password
            if (OldPassword == NewPassword)
            {
                ResultHandler?.Invoke(AccountResultCode.PswdNotChanged);
                return false;
            }


            try
            {
                if (!UserReader.DoesUserExist(UserID))
                {
                    ResultHandler?.Invoke(AccountResultCode.UserInvalid);
                    return false;
                }

                var modUser = UserReader.GetUser(UserID);
                if (modUser == null || modUser.ID == null)
                {
                    ResultHandler?.Invoke(AccountResultCode.UserInvalid);
                    return false;
                }


                var isAuthValid = CoreFactory.Singleton.Auth.TryAuthenticateUser(modUser.Email, OldPassword);
                if (!isAuthValid)
                {
                    ResultHandler?.Invoke(AccountResultCode.LoginFailed);
                    return false;
                }

                //try to change password
                var salt = SysSec.Membership.GeneratePassword(SALT_LENGTH, 0);
                string hashedPsd = null;
                try
                {
                    hashedPsd = NewPassword.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("B6877027-52A2-41D4-949F-E47578305C44"));
                    return false;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("F70C21AA-2469-477A-9518-7CBFA7BC6F88"));
                    return false;
                }
                try
                {
                    UpdateUserPassword_DB(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("6D23ECC3-1D36-4F81-8EE6-9F334E97265F"));
                    return false;
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

                ResultHandler?.Invoke(AccountResultCode.Success);
                return true;

            }
            catch (Exception ex)
            {
                var errCode = "B9932471-7779-4710-A97E-BB1FA147A995";
                Exception ex_outer = new Exception(errCode, ex);

                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return false;
            }
        }

        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="InvalidUserHandler">Error handler in case user does not exist</param>
        /// <param name="InvalidPasswordHandler">Error handler in case new password is invalid</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler"></param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        internal static bool TryResetPassword(
            string UserEmail,
            string NewPassword,
            Action<AccountResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (!CoreFactory.Singleton.Properties.EnablePswdReset)
            {
                throw new InvalidOperationException("Password resets are not enabled");
            }

            if (UserEmail.IsEmpty())
            {
                ResultHandler?.Invoke(AccountResultCode.EmailEmpty);
                return false;
            }

            UserEmail = UserEmail.Trim();

            var ID = UserReader.GetUserID(UserEmail, UserRefType.Email);
            if (ID == null)
            {
                ResultHandler?.Invoke(AccountResultCode.UserInvalid);
                return false;
            }

            return TryResetPassword(
                ID.Value,
                NewPassword,
                ResultHandler,
                GeneralFailHandler);
        }

        /// <summary>
        /// Attempts to reset a user password.  System level function that overrides validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="NewPassword">New password</param>
        /// <param name="InvalidUserHandler">Error handler in case user does not exist</param>
        /// <param name="InvalidPasswordHandler">Error handler in case new password is invalid</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler"></param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        internal static bool TryResetPassword(
            long UserID,
            string NewPassword,
            Action<AccountResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!CoreFactory.Singleton.Properties.EnablePswdReset)
            {
                throw new InvalidOperationException("Password resets are not enabled");
            }

            //check for valid password
            if (!Regex.IsMatch(NewPassword, CoreFactory.Singleton.Properties.PswdStrengthRegex))
            {
                ResultHandler?.Invoke(AccountResultCode.PswdInvalid);
                return false;
            }


            try
            {
                if (!UserReader.DoesUserExist(UserID))
                {
                    ResultHandler?.Invoke(AccountResultCode.UserInvalid);
                    return false;
                }

                var modUser = UserReader.GetUser(UserID);
                if (modUser == null || modUser.ID == null)
                {
                    ResultHandler?.Invoke(AccountResultCode.UserInvalid);
                    return false;
                }


                //try to change password
                var salt = SysSec.Membership.GeneratePassword(SALT_LENGTH, 0);
                string hashedPsd = null;
                try
                {
                    var hashType = CoreFactory.Singleton.Properties.PswdHashType;
                    hashedPsd = NewPassword.GetCryptoHash(hashType, salt);
                }
                catch (Exception ex1)
                {
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex1);
                    GeneralFailHandler?.Invoke(new Guid("AA3E2DB3-5CCF-400D-8046-1D982E723F58"));
                    return false;
                }
                if (hashedPsd.IsEmpty())
                {
                    GeneralFailHandler?.Invoke(new Guid("798206EE-253A-41F8-BF1F-D5FAC1608D54"));
                    return false;
                }
                try
                {
                    UpdateUserPassword_DB(modUser.ID.Value, hashedPsd, salt);
                }
                catch
                {
                    GeneralFailHandler?.Invoke(new Guid("7A6840DA-B08B-4972-B85F-11B45B45E3B0"));
                    return false;
                }

                //if everything worked, increment user version to force global re auth
                modUser.UpdateVersion();

                //re auth current user to prevent lapse in service
                CoreFactory.Singleton.Auth.TrySetClientAuthToken(modUser.Email, NewPassword, false);

                //remove any recovery contexts
                modUser.GetRecoveryContext()?.Delete();

                ResultHandler?.Invoke(AccountResultCode.Success);
                return true;


            }
            catch (Exception ex2)
            {
                var errCode = "8641F1E3-B29B-4CC1-ABA5-90B8693625EE";
                Exception ex_outer = new Exception(errCode, ex2);

                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);
                GeneralFailHandler?.Invoke(new Guid(errCode));
                return false;

            }
        }



        /// <summary>
        /// Delete user by FriendlyID.
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="UserUID"></param>
        public static void DeleteUser(long UserID)
        {
            var modUser = UserReader.GetUser(UserID);
            _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user by UserRef
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="UserFriendlyID"></param>
        public static void DeleteUser(string UserRef, UserRefType UserRefType)
        {
            var modUser = UserReader.GetUser(UserRef, UserRefType);
            _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="CmsUser"></param>
        private static void _deleteUser(User CmsUser)
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

                CoreFactory.Singleton.Logging.CreateErrorLog(ex_outer);
                throw new Exception();
            }
        }

        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="InvalidEmailHandler">Error handler in case user email is invalid</param>
        /// <param name="InvalidUserHandler">Error handler in case user does not exist</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler"></param>
        /// <returns></returns>
        internal static void CreateUserRecoveryContext(
            string UserEmail,
            bool IsOptional = true,
            Action<AccountResultCode> ArgFailHandler = null,
            Action<Guid> GeneralFailHandler = null,
            Action<IUserRecoveryContext, string> SuccessHandler = null)
        {
            //check for valid email format
            if (!UserEmail.IsValidEmail())
            {
                ArgFailHandler?.Invoke(AccountResultCode.EmailInvalid);
                return;
            }


            long? id = UserReader.GetUserID(UserEmail, UserRefType.Email);

            if (id == null)
            {
                ArgFailHandler?.Invoke(AccountResultCode.UserInvalid);
                return;
            }


            CreateUserRecoveryContext(id.Value, IsOptional, ArgFailHandler, GeneralFailHandler, SuccessHandler);

        }

        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="InvalidUserHandler">Error handler in case user does not exist</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler"></param>
        /// <returns></returns>
        internal static void CreateUserRecoveryContext(
            long UserID,
            bool IsOptional = true,
            Action<AccountResultCode> ArgFailHandler = null,
            Action<Guid> GeneralFailHandler = null,
            Action<IUserRecoveryContext, string> SuccessHandler = null)
        {
            try
            {
                var cmsUser = UserReader.GetUser(UserID);

                if (cmsUser == null || cmsUser.ID == null)
                {
                    ArgFailHandler?.Invoke(AccountResultCode.UserInvalid);
                    return;
                }


                //prevent a new recovery context from being made if the user already has an active forced reset
                //a new recovery context would override the prior and allow a bypass
                var recoveryContext = cmsUser.GetRecoveryContext();
                if (recoveryContext != null && !recoveryContext.IsOptional)
                {
                    ArgFailHandler?.Invoke(AccountResultCode.UserInvalid);
                    return;
                }

                string recoveryKey = SysSec.Membership.GeneratePassword(R_KEY_LENGTH, 5);
                string hashedKey = recoveryKey.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType);


                var context = UserWriter.CreateRecoveryContext(UserID, hashedKey, true, true);

                if (context == null)
                {
                    GeneralFailHandler?.Invoke(new Guid("B2AA0C33-A7A5-4026-ADC1-687C8406E8F8"));
                    return;
                }

                SuccessHandler?.Invoke(context, recoveryKey);
            }
            catch
            {
                ArgFailHandler?.Invoke(AccountResultCode.UserInvalid);
                return;
            }

        }


        private static void UpdateUserPassword_DB(string UserEmail, string Password, string Salt)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!UserReader.DoesUserExist(UserEmail, UserRefType.Email))
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
