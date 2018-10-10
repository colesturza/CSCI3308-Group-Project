using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;

namespace UHub.CoreLib.Users.Interfaces
{
    /// <summary>
    /// CMS User publically exposable interface for self-referencing access
    /// </summary>
    public interface IUser_Private : IUser_Public
    {
        #region DataProperties
        /// <summary>
        /// User email
        /// </summary>
        string Email { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// User phone number
        /// </summary>
        string PhoneNumber { get; set; }
        /// <summary>
        /// School ID
        /// </summary>
        long? SchoolID { get; set; }

        #endregion DataProperties

    }
}
