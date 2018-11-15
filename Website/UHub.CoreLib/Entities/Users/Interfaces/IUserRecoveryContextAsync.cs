using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    /// <summary>
    /// CMS user account recovery context
    /// </summary>
    public partial interface IUserRecoveryContext
    {
        /// <summary>
        /// Increment the attempt count in DB
        /// </summary>
        Task<AccountResultCode> IncrementAttemptCountAsync();


        /// <summary>
        /// Delete this context to prevent it from being used again
        /// </summary>
        Task DeleteAsync();
    }
}
