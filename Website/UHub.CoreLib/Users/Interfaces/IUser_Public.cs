using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;

namespace UHub.CoreLib.Users.Interfaces
{
    /// <summary>
    /// CMS User publically exposable interface
    /// </summary>
    public interface IUser_Public
    {
        #region DataProperties
        /// <summary>
        /// Username
        /// </summary>
        [DataProperty]
        string Username { get; }
        /// <summary>
        /// User college major
        /// </summary>
        [DataProperty]
        string Major { get; set; }
        /// <summary>
        /// User college year
        /// </summary>
        [DataProperty]
        string Year { get; set; }
        /// <summary>
        /// User expected graduation date
        /// </summary>
        [DataProperty]
        string GradDate { get; set; }
        /// <summary>
        /// User company
        /// </summary>
        [DataProperty]
        string Company { get; set; }
        /// <summary>
        /// User job title
        /// </summary>
        [DataProperty]
        string JobTitle { get; set; }

        #endregion DataProperties


    }
}
