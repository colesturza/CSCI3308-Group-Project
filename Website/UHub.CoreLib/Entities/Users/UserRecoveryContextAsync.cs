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
        public async Task<AcctRecoveryResultCode> TryIncrementAttemptCountAsync()
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
                await UserWriter.LogFailedRecoveryContextAttemptAsync(this.RecoveryID);
                return AcctRecoveryResultCode.Success;
            }
            catch (Exception ex)
            {
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync("1030F56D-0B6B-473C-9732-C828981FC332", ex);
                return AcctRecoveryResultCode.UnknownError;
            }
#pragma warning restore
        }


        /// <summary>
        /// Delete this recovery context from the DB
        /// </summary>
        public async Task<bool> TryDeleteAsync()
        {
#pragma warning disable 612, 618

            try
            {
                await UserWriter.DeleteRecoveryContextAsync(this.RecoveryID);
                return true;
            }
            catch (Exception ex)
            {
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync("042119E4-FB51-49A9-866B-F78DCFA0C567", ex);
                return false;
            }
#pragma warning restore
        }

    }
}
