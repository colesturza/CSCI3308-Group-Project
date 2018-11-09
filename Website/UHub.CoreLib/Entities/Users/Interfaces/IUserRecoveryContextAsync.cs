using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    /// <summary>
    /// CMS user account recovery context
    /// </summary>
    public partial interface IUserRecoveryContext
    {

        /// <summary>
        /// Delete this context to prevent it from being used again
        /// </summary>
        Task DeleteAsync();
    }
}
