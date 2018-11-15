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
    
    internal sealed partial class UserRecoveryContext : DBEntityBase, IUserRecoveryContext
    {

        /// <summary>
        /// Increment the attempt count in DB
        /// </summary>
        public async Task<AccountResultCode> IncrementAttemptCountAsync()
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
            await UserWriter.TryLogFailedRecoveryContextAttemptAsync(this.RecoveryID);
            return AccountResultCode.Success;
        }


        /// <summary>
        /// Delete this recovery context from the DB
        /// </summary>
        public async Task DeleteAsync()
        {
            await UserWriter.TryDeleteRecoveryContextAsync(this.RecoveryID);
        }

    }
}
