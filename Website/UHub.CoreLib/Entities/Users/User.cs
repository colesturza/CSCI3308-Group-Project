using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.Management;

namespace UHub.CoreLib.Entities.Users
{

    [DataClass]
    public sealed partial class User : DBEntityBase, IUserCredential, IUser_C_Public, IUser_R_Private, IUser_U_Private
    {
        private const short USER_VERSION_LENGTH = 10;
        private const string CONFIRMATION_URL_FORMAT = "{0}/{1}";


        [DataProperty]
        public long? ID { get; set; }

        //NOT A DATA PROP
        public string Password { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public string RefUID { get; private set; }

        [DataProperty]
        public bool IsConfirmed { get; set; }

        [DataProperty]
        public bool IsApproved { get; set; }

        [DataProperty]
        public string Version { get; set; }

        [DataProperty]
        public bool IsAdmin { get; private set; } = false;

        [DataProperty]
        public string Email { get; set; }

        [DataProperty]
        public string Username { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public string PhoneNumber { get; set; }

        [DataProperty]
        public string Major { get; set; }

        [DataProperty]
        public string Year { get; set; }

        [DataProperty]
        public string GradDate { get; set; }

        [DataProperty]
        public string Company { get; set; }

        [DataProperty]
        public string JobTitle { get; set; }

        [DataProperty]
        public bool IsFinished { get; set; }

        [DataProperty]
        public long? SchoolID { get; set; }

        [DataProperty]
        public long CreatedBy { get; set; }



        public string GetConfirmationURL()
        {
            if (this.ID == null || this.RefUID.IsEmpty())
            {
                return "/";
            }

            var url = CoreFactory.Singleton.Properties.AcctConfirmURL;
            
            return string.Format(CONFIRMATION_URL_FORMAT, url, this.RefUID);
        }

        public IUserRecoveryContext GetRecoveryContext()
        {
            if(this.ID == null)
            {
                throw new InvalidOperationException("Cannot get recovery context of anon user");
            }
            return UserReader.GetRecoveryContext(this.ID.Value);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void UpdateVersion()
        {
            if(ID == null)
            {
                return;
            }

            var version = Membership.GeneratePassword(USER_VERSION_LENGTH, 0);
            //sterilize for token processing
            version = version.Replace('|', '0');

            UserWriter.UpdateUserVersion(this.ID.Value, version);

        }
    }
}
