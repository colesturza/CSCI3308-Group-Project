using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;

namespace UHub.CoreLib.Users.Interfaces
{
    /// <summary>
    /// CMS User internal interface. Should not ever be exposed to public API
    /// </summary>
    public interface IUser_Internal : IUser_Private
    {
        #region DataProperties
        /// <summary>
        /// User int ID
        /// </summary>
        long? UserID { get; }
        /// <summary>
        /// User enabled flag
        /// </summary>
        bool IsEnabled { get; set; }
        /// <summary>
        /// User readonly flag
        /// </summary>
        bool IsReadOnly { get; set; }
        /// <summary>
        /// User long reference ID - used to confirm account email
        /// </summary>
        string RefUID { get; }
        /// <summary>
        /// Is user email confirmed
        /// </summary>
        bool IsConfirmed { get; set; }
        /// <summary>
        /// Is user account approved
        /// </summary>
        bool IsApproved { get; set; }
        /// <summary>
        /// User authentication version. Used to invalidate old auth tokens
        /// </summary>
        string Version { get; set; }
        /// <summary>
        /// User admin status
        /// </summary>
        bool IsAdmin { get; }
        /// <summary>
        /// User creator
        /// </summary>
        long CreatedBy { get; set; }
        #endregion DataProperties



        #region Confirmation
        /// <summary>
        /// Get link for account confirmation
        /// </summary>
        /// <returns></returns>
        string GetConfirmationURL();
        #endregion Confirmation

        #region Save
        /// <summary>
        /// Save user account to DB
        /// </summary>
        void Save();

        /// <summary>
        /// Update user auth version
        /// </summary>
        void UpdateVersion();
        #endregion Save


        #region Recovery
        /// <summary>
        /// Check for account password recovery context (either voluntary or forced password change)
        /// </summary>
        /// <returns></returns>
        IUserRecoveryContext GetRecoveryContext();
        #endregion Recovery
    }
}
