using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Security.Authentication
{
    [DataClass]
    internal sealed partial class UserAuthInfo : CustomDBTypeBase
    {

        #region DataProperties
        [DataProperty]
        internal long UserID { get; private set; }
        [DataProperty]
        internal string PswdHash { get; private set; }
        [DataProperty]
        internal string Salt { get; private set; }
        [DataProperty]
        internal DateTimeOffset PswdCreatedDate { get; private set; }
        [DataProperty]
        internal DateTimeOffset PswdModifiedDate { get; private set; }
        [DataProperty]
        internal DateTimeOffset? LastLockoutDate { get; private set; }
        [DataProperty]
        internal DateTimeOffset? StartFailedPswdWindow { get; private set; }
        [DataProperty]
        internal byte FailedPswdAttemptCount { get; private set; }
        #endregion DataProperties


        #region StandardProperties
        public bool IsLockedOut
        {
            get
            {
                var maxPsdAttmpt = CoreFactory.Singleton.Properties.MaxPswdAttempts;

                var lock1 = LastLockoutDate != null;
                var lock2 = lock1 && FailedPswdAttemptCount >= maxPsdAttmpt;

                return lock2;
            }
        }
        #endregion


    }
}
