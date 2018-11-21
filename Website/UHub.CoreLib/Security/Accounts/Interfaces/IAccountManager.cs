﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;

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
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler">Args: new user object, auto login [T|F]</param>
        /// <returns>Status Flag</returns>
        AcctCreateResultCode TryCreateUser(
            User NewUser,
            bool AttemptAutoLogin,
            Action<Guid> GeneralFailHandler = null,
            Action<User, bool> SuccessHandler = null);


        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        AcctConfirmResultCode TryConfirmUser(string RefUID);



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
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        AcctPswdResultCode TryUpdatePassword(
            string UserEmail,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            Action<Guid> GeneralFailHandler = null);
        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        AcctPswdResultCode TryUpdatePassword(
            long UserID,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            Action<Guid> GeneralFailHandler = null);



        /// <summary>
        /// Attempt to recover account password using a recovery context ID and key
        /// </summary>
        /// <param name="RecoveryContextID"></param>
        /// <param name="RecoveryKey"></param>
        /// <param name="NewPassword"></param>
        /// <param name="DeviceLogout"></param>
        /// <param name="GeneralFailHandler"></param>
        /// <returns></returns>
        AcctRecoveryResultCode TryRecoverPassword(
            string RecoveryContextID,
            string RecoveryKey,
            string NewPassword,
            bool DeviceLogout,
            Action<Guid> GeneralFailHandler = null);



        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout"></param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        AcctRecoveryResultCode TryResetPassword(
            string UserEmail,
            string NewPassword,
            bool DeviceLogout,
            Action<Guid> GeneralFailHandler = null);
        /// <summary>
        /// Attempts to reset a user password.  System level function that overrides validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="NewPassword">New password</param>
        /// <param name="DeviceLogout"></param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        AcctRecoveryResultCode TryResetPassword(
            long UserID,
            string NewPassword,
            bool DeviceLogout,
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


        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        (AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey) TryCreateUserRecoveryContext(
            string UserEmail,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null);


        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        (AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey) TryCreateUserRecoveryContext(
            long UserID,
            bool IsOptional,
            Action<Guid> GeneralFailHandler = null);
    }
}
