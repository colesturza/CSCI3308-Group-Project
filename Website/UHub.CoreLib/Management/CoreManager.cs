using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Config;
using UHub.CoreLib.EmailInterop.Providers;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Logging.Management;
using UHub.CoreLib.Logging.Providers;
using UHub.CoreLib.Security;
using UHub.CoreLib.Security.Accounts;
using UHub.CoreLib.Security.Accounts.Interfaces;
using UHub.CoreLib.Security.Accounts.Management;
using UHub.CoreLib.Security.Authentication;
using UHub.CoreLib.Security.Authentication.Interfaces;
using UHub.CoreLib.Security.Authentication.Management;

namespace UHub.CoreLib.Management
{
    public class CoreManager
    {


        public bool IsEnabled { get; } = true;


        //PROPERTIES
        private CoreProperties _properties;
        public CoreProperties Properties { get => _properties; }


        //SECURITY
        private RecaptchaManager _recaptcha;
        public RecaptchaManager Recaptcha { get => _recaptcha; }
        //auth
        private IAuthenticationManager _auth;
        public IAuthenticationManager Auth { get => _auth; }
        //account
        private IAccountManager _account;
        public IAccountManager Accounts { get => _account; }


        //LOGGING
        private LoggingManager _logging;
        public LoggingManager Logging { get => _logging; }

        //MAIL
        private EmailProvider _mail;
        public EmailProvider Mail { get => _mail; }


        /// <summary>
        ///Define constructor to isolate access
        /// </summary>
        internal protected CoreManager(CmsConfiguration_Grouped cmsConfig)
        {

            cmsConfig.Validate();



            _properties = (CoreProperties)cmsConfig;
            if (!_properties.CmsSchemaVersion.Validate(_properties.CmsDBConfig))
            {
                throw new InvalidOperationException("This version of the CMS Manager does not support the specified DB schema.");
            }



            //------------------LOGGING------------------
            _logging = new LoggingManager(_properties);



            _recaptcha = new RecaptchaManager();


            _auth = new AuthenticationManager();
            _account = new AccountManager();

            _mail = Properties.MailProvider;


            System.Web.Http.GlobalConfiguration.Configure((config) =>
            {
                config.EnsureInitialized();
            });

        }




    }
}
