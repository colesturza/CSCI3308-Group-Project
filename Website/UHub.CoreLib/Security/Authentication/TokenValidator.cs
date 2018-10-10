using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Security.Authentication
{
    [DataClass]
    internal sealed partial class TokenValidator : CustomDBTypeBase
    {

        #region DataProperties
        [DataProperty]
        internal DateTimeOffset IssueDate { get; private set; }
        [DataProperty]
        internal DateTimeOffset MaxExpirationDate { get; private set; }
        [DataProperty]
        internal string TokenID { get; private set; }
        [DataProperty]
        internal bool IsPersistent { get; private set; }
        [DataProperty]
        internal string TokenHash { get; private set; }
        [DataProperty]
        internal string RequestID { get; private set; }
        [DataProperty]
        internal string SessionID { get; private set; }
        [DataProperty]
        internal bool IsValid { get; private set; }
        #endregion DataProperties
    }
}
