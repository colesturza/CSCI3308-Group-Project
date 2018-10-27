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
    public sealed partial class User : DBEntityBase, IUser_C_Public, IUser_R_Private, IUser_U_Private
    {


        [DataProperty(EnableDBColumnValidation: false)]
        public long? ID { get; set; }

        //NOT A DATA PROP
        public string Password { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsEnabled { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsReadOnly { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string RefUID { get; private set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsConfirmed { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsApproved { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Version { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsAdmin { get; private set; } = false;

        [DataProperty(EnableDBColumnValidation: false)]
        public string Email { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Username { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Name { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string PhoneNumber { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Major { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Year { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string GradDate { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Company { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string JobTitle { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsFinished { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public long? SchoolID { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public long CreatedBy { get; set; }



        public string GetConfirmationURL()
        {
            if (this.ID == null || this.RefUID.IsEmpty())
            {
                return "/";
            }
            return CoreFactory.Singleton.Properties.AcctConfirmURL + $"?ID={this.RefUID}";
        }

        public IUserRecoveryContext GetRecoveryContext()
        {
            if(this.ID == null)
            {
                throw new InvalidOperationException("Cannot get recovery context of anon user");
            }
            return UserReader.GetUserRecoveryContext(this.ID.Value);
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

            var version = Membership.GeneratePassword(20, 0);
            UserWriter.UpdateUserVersion(this.ID.Value, version);

        }
    }
}
