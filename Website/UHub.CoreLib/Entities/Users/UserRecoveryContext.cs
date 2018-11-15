using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.Management;
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Entities.Users
{
    [DataClass]
    internal sealed partial class UserRecoveryContext : DBEntityBase, IUserRecoveryContext
    {
        private const string RECOVER_URL_FORMAT = "{0}/{1}";


        #region DataProperties
        [DataProperty]
        public long UserID { get; set; }
        [DataProperty]
        public string RecoveryID { get; set; }
        [DataProperty]
        public string RecoveryKey { get; set; }
        [DataProperty]
        public DateTimeOffset EffFromDate { get; set; }
        [DataProperty]
        public DateTimeOffset EffToDate { get; set; }
        [DataProperty]
        public bool IsEnabled { get; set; }
        [DataProperty]
        public byte AttemptCount { get; set; }
        [DataProperty]
        public bool IsOptional { get; set; }
        #endregion DataProperties


        #region StdProperties
        /// <summary>
        /// Get absolute path to recovery page, including RecoveryID
        /// </summary>
        public string RecoveryURL
        {
            get
            {
                var url = CoreFactory.Singleton.Properties.AcctPswdRecoveryURL;

                return string.Format(RECOVER_URL_FORMAT, url, this.RecoveryID);
            }
        }
        #endregion StdProperties


        #region Constructors
        internal UserRecoveryContext()
        {

        }
        #endregion Constructors


        public AccountResultCode ValidateRecoveryKey(string Key)
        {

            if (this.EffFromDate > DateTimeOffset.Now || this.EffToDate < DateTimeOffset.Now)
            {
                this.Delete();
                return AccountResultCode.RecoveryContextExpired;
            }

            if (!this.IsEnabled)
            {
                return AccountResultCode.RecoveryContextDisabled;
            }

            if (this.AttemptCount > CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                this.Delete();
                return AccountResultCode.RecoveryContextDestroyed;
            }


            bool isValid = false;
            if (CoreFactory.Singleton.Properties.PswdHashType == CryptoHashType.Bcrypt)
            {
                isValid = BCrypt.Net.BCrypt.Verify(Key, this.RecoveryKey);
            }
            else
            {
                isValid = this.RecoveryKey == Key.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType);
            }


            if (isValid)
            {
                return AccountResultCode.Success;
            }
            else
            {
                return AccountResultCode.RecoveryKeyInvalid;
            }
        }


        /// <summary>
        /// Increment the attempt count in DB
        /// </summary>
        public AccountResultCode IncrementAttemptCount()
        {
            if (AttemptCount >= CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                this.Delete();
                return AccountResultCode.RecoveryContextDestroyed;
            }

            //Forced reset does not respect attempt count
            //But no error should be reported
            if (!this.IsOptional)
            {
                return AccountResultCode.Success;
            }


            this.AttemptCount++;
            UserWriter.LogFailedRecoveryContextAttempt(this.RecoveryID);
            return AccountResultCode.Success;
        }

        /// <summary>
        /// Delete this recovery context from the DB
        /// </summary>
        public void Delete()
        {
            UserWriter.DeleteRecoveryContext(this.RecoveryID);
        }

    }
}
