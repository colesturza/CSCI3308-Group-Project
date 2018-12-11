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
using UHub.CoreLib.Entities.Users.DataInterop;
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


        public AcctRecoveryResultCode ValidateRecoveryKey(string Key)
        {

            if (this.EffFromDate > DateTimeOffset.Now || this.EffToDate < DateTimeOffset.Now)
            {
                this.TryDelete();
                return AcctRecoveryResultCode.RecoveryContextExpired;
            }

            if (!this.IsEnabled)
            {
                return AcctRecoveryResultCode.RecoveryContextDisabled;
            }

            if (this.AttemptCount > CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                this.TryDelete();
                return AcctRecoveryResultCode.RecoveryContextDestroyed;
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
                return AcctRecoveryResultCode.Success;
            }
            else
            {
                return AcctRecoveryResultCode.RecoveryKeyInvalid;
            }
        }


        /// <summary>
        /// Increment the attempt count in DB
        /// </summary>
        public AcctRecoveryResultCode TryIncrementAttemptCount()
        {
#pragma warning disable 612, 618
            if (AttemptCount >= CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                this.TryDelete();
                return AcctRecoveryResultCode.RecoveryContextDestroyed;
            }

            //Forced reset does not respect attempt count
            //But no error should be reported
            if (!this.IsOptional)
            {
                return AcctRecoveryResultCode.Success;
            }


            this.AttemptCount++;
            try
            {
                UserWriter.LogFailedRecoveryContextAttempt(this.RecoveryID);
                return AcctRecoveryResultCode.Success;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, "320E32EC-416F-4627-ADA1-D38EA201FCF0");
                return AcctRecoveryResultCode.UnknownError;
            }
#pragma warning restore
        }

        /// <summary>
        /// Delete this recovery context from the DB
        /// </summary>
        public bool TryDelete()
        {
#pragma warning disable 612, 618
            try
            {
                UserWriter.DeleteRecoveryContext(this.RecoveryID);
                return true;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, "AEB010D9-AF61-4108-9C5F-448CCAFE9EA8");
                return false;
            }
#pragma warning restore
        }

    }
}
