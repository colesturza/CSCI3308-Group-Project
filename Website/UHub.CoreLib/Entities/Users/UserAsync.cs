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

    public partial class User
    {
       
        public async Task<IUserRecoveryContext> GetRecoveryContextAsync()
        {
            if(this.ID == null)
            {
                throw new InvalidOperationException("Cannot get recovery context of anon user");
            }
            return await UserReader.GetUserRecoveryContextAsync(this.ID.Value);
        }

        public async Task<bool> SaveAsync()
        {
            if (this.ID == null)
            {
                throw new InvalidOperationException("Cannot save anon user");
            }

            return await UserWriter.TryUpdateUserInfoAsync(this);

        }

        public async Task UpdateVersionAsync()
        {
            if(ID == null)
            {
                throw new InvalidOperationException("Cannot update version of anon user");
            }

            var version = Membership.GeneratePassword(USER_VERSION_LENGTH, 0);
            //sterilize for token processing
            version = version.Replace('|', '0');

            await UserWriter.TryUpdateUserVersionAsync(this.ID.Value, version);

        }
    }
}
