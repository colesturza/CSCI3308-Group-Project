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

namespace UHub.CoreLib.Entities.Users
{
    [DataClass]
    internal sealed partial class UserRecoveryContext : DBEntityBase, IUserRecoveryContext
    {
        #region DataProperties
        [DataProperty]
        public long UserID { get; set; }
        [DataProperty]
        public Guid UserUID { get; set; }
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

        public string RecoveryURL
        {
            get => CoreFactory.Singleton.Properties.AcctPswdResetURL + "?ID=" + this.RecoveryID;
        }
        #endregion StdProperties


        #region Constructors
        internal UserRecoveryContext()
        {

        }
        #endregion Constructors


        public bool ValidateRecoveryKey(string Key)
        {

            if (this.EffFromDate > DateTimeOffset.Now || this.EffToDate < DateTimeOffset.Now)
            {
                this.Delete();
                return false;
            }

            if (!this.IsEnabled)
            {
                return false;
            }

            if (this.AttemptCount > CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                this.Delete();
                return false;
            }

            if (CoreFactory.Singleton.Properties.PswdHashType == CryptoHashType.Bcrypt)
            {
                return BCrypt.Net.BCrypt.Verify(Key, this.RecoveryKey);
            }
            else
            {
                return this.RecoveryKey == Key.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType);
            }
        }


        /// <summary>
        /// Increment the attempt count in DB
        /// </summary>
        public void UpdateAttemptCount()
        {
            if (AttemptCount >= CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                this.Delete();
                throw new InvalidOperationException("This recovery context has exceeded its maximum attempt count");
            }

            //forced reset does not respect attempt count
            if (!this.IsOptional)
            {
                throw new InvalidOperationException("This recovery context does not respect attempt count");
            }

            this.AttemptCount++;
            UserWriter.LogFailedRecoveryContextAttempt(this.RecoveryID);
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
