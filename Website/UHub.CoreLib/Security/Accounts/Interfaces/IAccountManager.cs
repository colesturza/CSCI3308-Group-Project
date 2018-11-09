using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users;

namespace UHub.CoreLib.Security.Accounts.Interfaces
{
    public partial interface IAccountManager
    {
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
        bool TryCreateUser(
            User NewUser,
            bool AttemptAutoLogin,
            out AccountResultCode ResultCode,
            Action<Guid> GeneralFailHandler = null,
            Action<User, bool> SuccessHandler = null);


        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        bool TryConfirmUser(string RefUID);
        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        bool TryConfirmUser(string RefUID, out string Status);
        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        bool ConfirmUser(string RefUID, out string Status);



        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        bool TryUpdateApprovalStatus(long UserID, bool IsApproved);
        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        void UpdateUserApprovalStatus(long UserID, bool IsApproved);



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
        bool TryUpdatePassword(
            string UserEmail,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            out AccountResultCode ResultCode,
            Action<Guid> GeneralFailHandler = null);
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
        bool TryUpdatePassword(
            long UserID,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            out AccountResultCode ResultCode,
            Action<Guid> GeneralFailHandler = null);



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
        bool TryResetPassword(
            string UserEmail,
            string NewPassword,
            out AccountResultCode ResultCode,
            Action<Guid> GeneralFailHandler = null);
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
        bool TryResetPassword(
        long UserID,
            string NewPassword,
            out AccountResultCode ResultCode,
            Action<Guid> GeneralFailHandler = null);



        /// <summary>
        /// Delete user by ID.
        /// </summary>
        /// <param name="UserID"></param>
        void DeleteUser(long UserID);
        /// <summary>
        /// Delete user by Email
        /// </summary>
        /// <param name="Email"></param>
        void DeleteUser(string Email);
        /// <summary>
        /// Delete user by Username and Domain
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        void DeleteUser(string Username, string Domain);
    }
}
