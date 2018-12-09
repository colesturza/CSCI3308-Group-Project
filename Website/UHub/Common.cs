using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using UHub.CoreLib.Config;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Security;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.EmailInterop.Providers.SMTP;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;

namespace UHub
{
    public static class Common
    {


        public static CmsConfiguration_Grouped GetCmsConfig()
        {
            //ENV
            var env = WebConfigurationManager.AppSettings["Environment"];
            //DB
            var dbConn = WebConfigurationManager.AppSettings["DB_CONN"];



            //test for temp config
            if (dbConn == "TEST")
            {
                //if config is bad, load real config from server cache
                //this will update the web.config file
                //changes to this file initiate a website restart
                using (PowerShell shellCmd = PowerShell.Create())
                {
                    shellCmd.AddCommand(@"D:\IISData\UHUB\_configs\Migrate_Dev.ps1");
                    shellCmd.AddCommand(@"D:\IISData\UHUB\_configs\Migrate_Prd.ps1");
                    shellCmd.Invoke();
                }
                return null;
            }


            if (env == "PRD")
            {
                return GetPrdConfig();
            }
            else if (env == "DEV")
            {
                return GetDevConfig();
            }
            else
            {
                return GetDevConfig();
            }

        }

        private static CmsConfiguration_Grouped GetPrdConfig()
        {
            var domain = "u-hub.life";


            //ENV
            var env = WebConfigurationManager.AppSettings["Environment"];

            //DB
            var dbConn = WebConfigurationManager.AppSettings["DB_CONN"];

            //SECURITY
            var autoApproveAccts = bool.Parse(WebConfigurationManager.AppSettings["AutoApproveNewUsers"]);

            //MAIL
            var ContactFormRecipient = WebConfigurationManager.AppSettings["ContactFormRecipient"];
            var mailFromAddr = WebConfigurationManager.AppSettings["MailFromAddress"];
            var mailFromName = WebConfigurationManager.AppSettings["MailFromName"];
            var mailHost = WebConfigurationManager.AppSettings["MailHost"];
            var mailPort = int.Parse(WebConfigurationManager.AppSettings["MailPort"]);
            var mailUsername = WebConfigurationManager.AppSettings["MailUsername"];
            var mailPswd = WebConfigurationManager.AppSettings["MailPassword"];

            //FILES
            var fileStoreDir = WebConfigurationManager.AppSettings["FileStoreDirectory"];
            var imgStoreDir = WebConfigurationManager.AppSettings["ImageStoreDirectory"];
            var tempCacheDir = WebConfigurationManager.AppSettings["TempCacheDirectory"];
            var logStoreDir = WebConfigurationManager.AppSettings["LogStoreDirectory"];

            //CAPTCHA
            var captchaPublicKey = WebConfigurationManager.AppSettings["RecaptchaPublicKey"];
            var captchaPrivateKey = WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];

            //LOGGING
            var googleAnalyticsKey = WebConfigurationManager.AppSettings["GoogleAnalyticsKey"];



            var allowedFileTypes = new FileCategory[]
                    {
                        FileCategory.Image,
                        FileCategory.Document
                    };

            var mailConfig = new SmtpConfig(new MailAddress(mailFromAddr, mailFromName), false, true, mailHost, mailPort, mailUsername, mailPswd);





            var cmsConfig = new CmsConfiguration_Grouped()
            {
                Instance = new CmsConfig_Instance
                {
                    SiteFriendlyName = "UHUB",
                    CmsPublicBaseURL = $"https://{domain}",
                    CmsStaticResourceURL = $"https://{domain}",
                },
                DB = new CmsConfig_DB
                {
                    CmsDBConfig = new SqlConfig(dbConn),
                    EnableDBMultithreading = true
                },
                Storage = new CmsConfig_Storage
                {
                    FileStoreDirectory = fileStoreDir,
                    ImageStoreDirectory = imgStoreDir,
                    TempCacheDirectory = tempCacheDir,
                    LogStoreDirectory = logStoreDir,
                    CreateMissingStorageDirectories = true
                },
                Mail = new CmsConfig_Mail
                {
                    MailProvider = new SmtpProvider(mailConfig),
                    ContactFormRecipientAddress = ContactFormRecipient
                },
                Security = new CmsConfig_Security
                {
                    PswdHashType = CryptoHashType.HMACSHA512,
                    ForceHTTPS = true,
                    ForceSecureCookies = true,
                    CookieDomain = domain,
                    ForceSecureResponseHeaders = true,
                    AuthTokenTimeout = new TimeSpan(0, 6, 0, 0),
                    MaxAuthTokenLifespan = new TimeSpan(30, 0, 0, 0),
                    EnableAuthTokenSlidingExpiration = true,
                    LoginURL = "~/Account/Login",
                    DefaultAuthFwdURL = "~/School",
                    AcctConfirmURL = "~/Account/Confirm",
                    AcctPswdRecoveryURL = "~/Account/Recover",
                    AcctPswdUpdateURL = "~/Account/UpdatePassword",
                    AcctPswdRecoveryLifespan = new TimeSpan(0, 0, 30, 0),
                    EnableRecaptcha = true,                                         //CAPTCHA
                    RecaptchaPublicKey = captchaPublicKey,
                    RecaptchaPrivateKey = captchaPrivateKey,
                    AutoConfirmNewAccounts = false,
                    AutoApproveNewAccounts = autoApproveAccts,
                    EnableTokenVersioning = true,                                   //VERSION
                    CookieSameSiteMode = CookieSameSiteModes.Lax,
                    EnablePswdRecovery = true,
                    EnablePersistentAuthTokens = true,
                    HtmlSanitizerMode = HtmlSanitizerMode.Both
                },
                Logging = new CmsConfig_Logging
                {
                    //EnableUserSessionLogging = true,
                    //EnableUserActivityLogging = true,
                    EventLogMode = EventLoggingMode.SystemEvents,
                    LoggingSource = LocalSysLoggingSource.UHUB_CMS,
                    UsageLogMode = UsageLoggingMode.None,
                    GoogleAnalyticsKey = googleAnalyticsKey
                },
                Errors = new CmsConfig_Errors
                {
                    EnableCustomErrorCodes = true
                },
                Caching = new CmsConfig_Caching
                {
                    EnableStartupCachePopulation = true,
                    EnableNavBarCaching = true,
                    EnableDBPageCaching = false,
                    MaxDBCacheAge = new TimeSpan(0, 6, 0, 0),
                    EnableIISPageCaching = false,
                    MaxDynamicCacheAge = new TimeSpan(0, 1, 0, 0),
                    MaxStaticCacheAge = new TimeSpan(1, 0, 0, 0),
                },
                API = new CmsConfig_API
                {
                    RegisterAPIRoutes = true,
                    EnableDetailedAPIErrors = true,
                    EnableAPIAuthService = true,
                    EnableAPIFileUploads = true,
                    MaxFileUploadSize = new FileSize(FileSizeUnit.Gibibyte, 1),
                    AllowedFileUploadTypes = allowedFileTypes
                },
            };


            return cmsConfig;
        }




        private static CmsConfiguration_Grouped GetDevConfig()
        {
            var domain = "dev.u-hub.life";


            //ENV
            var env = WebConfigurationManager.AppSettings["Environment"];

            //DB
            var dbConn = WebConfigurationManager.AppSettings["DB_CONN"];

            //SECURITY
            var autoApproveAccts = bool.Parse(WebConfigurationManager.AppSettings["AutoApproveNewUsers"]);

            //MAIL
            var ContactFormRecipient = WebConfigurationManager.AppSettings["ContactFormRecipient"];
            var mailFromAddr = WebConfigurationManager.AppSettings["MailFromAddress"];
            var mailFromName = WebConfigurationManager.AppSettings["MailFromName"];
            var mailHost = WebConfigurationManager.AppSettings["MailHost"];
            var mailPort = int.Parse(WebConfigurationManager.AppSettings["MailPort"]);
            var mailUsername = WebConfigurationManager.AppSettings["MailUsername"];
            var mailPswd = WebConfigurationManager.AppSettings["MailPassword"];

            //FILES
            var fileStoreDir = WebConfigurationManager.AppSettings["FileStoreDirectory"];
            var imgStoreDir = WebConfigurationManager.AppSettings["ImageStoreDirectory"];
            var tempCacheDir = WebConfigurationManager.AppSettings["TempCacheDirectory"];
            var logStoreDir = WebConfigurationManager.AppSettings["LogStoreDirectory"];

            //CAPTCHA
            var captchaPublicKey = WebConfigurationManager.AppSettings["RecaptchaPublicKey"];
            var captchaPrivateKey = WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];

            //LOGGING
            var googleAnalyticsKey = WebConfigurationManager.AppSettings["GoogleAnalyticsKey"];



            var allowedFileTypes = new FileCategory[]
                    {
                        FileCategory.Image,
                        FileCategory.Document
                    };

            var mailConfig = new SmtpConfig(new MailAddress(mailFromAddr, mailFromName), false, true, mailHost, mailPort, mailUsername, mailPswd);





            var cmsConfig = new CmsConfiguration_Grouped()
            {
                Instance = new CmsConfig_Instance
                {
                    SiteFriendlyName = "UHUB",
                    CmsPublicBaseURL = $"https://{domain}",
                    CmsStaticResourceURL = $"https://{domain}",
                },
                DB = new CmsConfig_DB
                {
                    CmsDBConfig = new SqlConfig(dbConn),
                    EnableDBMultithreading = true
                },
                Storage = new CmsConfig_Storage
                {
                    FileStoreDirectory = fileStoreDir,
                    ImageStoreDirectory = imgStoreDir,
                    TempCacheDirectory = tempCacheDir,
                    LogStoreDirectory = logStoreDir,
                    CreateMissingStorageDirectories = true
                },
                Mail = new CmsConfig_Mail
                {
                    MailProvider = new SmtpProvider(mailConfig),
                    ContactFormRecipientAddress = ContactFormRecipient
                },
                Security = new CmsConfig_Security
                {
                    PswdHashType = CryptoHashType.HMACSHA512,
                    ForceHTTPS = true,
                    ForceSecureCookies = true,
                    CookieDomain = domain,
                    ForceSecureResponseHeaders = true,
                    AuthTokenTimeout = new TimeSpan(0, 6, 0, 0),
                    MaxAuthTokenLifespan = new TimeSpan(30, 0, 0, 0),
                    EnableAuthTokenSlidingExpiration = true,
                    LoginURL = "~/Account/Login",
                    DefaultAuthFwdURL = "~/School",
                    AcctConfirmURL = "~/Account/Confirm",
                    AcctPswdRecoveryURL = "~/Account/Recover",
                    AcctPswdUpdateURL = "~/Account/UpdatePassword",
                    AcctPswdRecoveryLifespan = new TimeSpan(0, 0, 30, 0),
                    EnableRecaptcha = true,                                         //CAPTCHA
                    RecaptchaPublicKey = captchaPublicKey,
                    RecaptchaPrivateKey = captchaPrivateKey,
                    AutoConfirmNewAccounts = false,
                    AutoApproveNewAccounts = autoApproveAccts,
                    EnableTokenVersioning = false,                                  //VERSION
                    CookieSameSiteMode = CookieSameSiteModes.Lax,
                    EnablePswdRecovery = true,
                    EnablePersistentAuthTokens = true,
                    HtmlSanitizerMode = HtmlSanitizerMode.Both
                },
                Logging = new CmsConfig_Logging
                {
                    //EnableUserSessionLogging = true,
                    //EnableUserActivityLogging = true,
                    EventLogMode = EventLoggingMode.SystemEvents,
                    LoggingSource = LocalSysLoggingSource.UHUB_CMS,
                    UsageLogMode = UsageLoggingMode.None,
                    GoogleAnalyticsKey = googleAnalyticsKey
                },
                Errors = new CmsConfig_Errors
                {
                    EnableCustomErrorCodes = true
                },
                Caching = new CmsConfig_Caching
                {
                    EnableStartupCachePopulation = true,
                    EnableNavBarCaching = true,
                    EnableDBPageCaching = false,
                    MaxDBCacheAge = new TimeSpan(0, 6, 0, 0),
                    EnableIISPageCaching = false,
                    MaxDynamicCacheAge = new TimeSpan(0, 1, 0, 0),
                    MaxStaticCacheAge = new TimeSpan(1, 0, 0, 0),
                },
                API = new CmsConfig_API
                {
                    RegisterAPIRoutes = true,
                    EnableDetailedAPIErrors = true,
                    EnableAPIAuthService = true,
                    EnableAPIFileUploads = true,
                    MaxFileUploadSize = new FileSize(FileSizeUnit.Gibibyte, 1),
                    AllowedFileUploadTypes = allowedFileTypes
                },
            };


            return cmsConfig;
        }

    }
}