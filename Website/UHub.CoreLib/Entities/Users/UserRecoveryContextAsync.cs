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
    
    internal sealed partial class UserRecoveryContext : DBEntityBase, IUserRecoveryContext
    {

        /// <summary>
        /// Increment the attempt count in DB
        /// </summary>
        public async Task<AcctRecoveryResultCode> IncrementAttemptCountAsync()
        {
#pragma warning disable 612, 618
            if (AttemptCount >= CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                this.Delete();
                return AcctRecoveryResultCode.RecoveryContextDestroyed;
            }

            //Forced reset does not respect attempt count
            //But no error should be reported
            if (!this.IsOptional)
            {
                return AcctRecoveryResultCode.Success;
            }


            this.AttemptCount++;
            await UserWriter.TryLogFailedRecoveryContextAttemptAsync(this.RecoveryID);
            return AcctRecoveryResultCode.Success;
#pragma warning restore
        }


        /// <summary>
        /// Delete this recovery context from the DB
        /// </summary>
        public async Task DeleteAsync()
        {
#pragma warning disable 612, 618
            await UserWriter.TryDeleteRecoveryContextAsync(this.RecoveryID);
#pragma warning restore
        }

    }
}
