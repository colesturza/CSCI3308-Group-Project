using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Users
{
    [DataClass]
    public sealed partial class UserConfirmToken : DBEntityBase, IUserConfirmToken
    {
        private const string CONFIRMATION_URL_FORMAT = "{0}/{1}";


        [DataProperty]
        public long UserID { get; set; }

        [DataProperty]
        public string RefUID { get; set; }

        [DataProperty]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty]
        public DateTimeOffset ConfirmedDate{ get; set; }

        [DataProperty]
        public bool IsConfirmed { get; set; }

        [DataProperty]
        public bool IsDeleted { get; set; }


        public string GetURL()
        {
            if (this.RefUID.IsEmpty())
            {
                return "/";
            }

            var url = CoreFactory.Singleton.Properties.AcctConfirmURL;

            return string.Format(CONFIRMATION_URL_FORMAT, url, this.RefUID);
        }
    }
}
