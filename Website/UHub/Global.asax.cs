using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Configuration;
using UHub.CoreLib.Config;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.SmtpInterop;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;

namespace UHub
{
    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Start()
        {
            //DB
            var dbConn = WebConfigurationManager.AppSettings["DB_CONN"];

            //MAIL
            var ContactFormRecipient = WebConfigurationManager.AppSettings["ContactFormRecipient"];
            var mailFromAddr = WebConfigurationManager.AppSettings["MailFromAddress"];
            var mailFromName = WebConfigurationManager.AppSettings["MailFromName"];
            var mailHost = WebConfigurationManager.AppSettings["MailHost"];
            var mailPort = int.Parse(WebConfigurationManager.AppSettings["MailPort"]);
            var mailUsername = WebConfigurationManager.AppSettings["MailUsername"];
            var mailPswd = WebConfigurationManager.AppSettings["MailPassword"];


            //FILES
            var fileStoreDir = WebConfigurationManager.AppSettings["FileStoreDir"];
            var imgStoreDir = WebConfigurationManager.AppSettings["ImageStoreDirectory"];
            var tempCacheDir = WebConfigurationManager.AppSettings["TempCacheDirectory"];
            var logStoreDir = WebConfigurationManager.AppSettings["LogStoreDirectory"];


            //CAPTCHA
            var captchaPublicKey = WebConfigurationManager.AppSettings["RecaptchaPublicKey"];
            var captchaPrivateKey = WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];




            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);




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
                    CmsPublicBaseURL = "https://u-hub.life",
                    CmsStaticResourceURL = "https://static.u-hub.life",
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
                    NoReplyMailConfig = mailConfig,
                    ContactFormRecipientAddress = ContactFormRecipient
                },
                Security = new CmsConfig_Security
                {
                    PswdHashType = CryptoHashType.HMACSHA512,
                    ForceHTTPS = true,
                    ForceSecureCookies = true,
                    CookieDomain = ".u-hub.life",
                    ForceSecureResponseHeaders = true,
                    AuthTokenTimeout = new TimeSpan(0, 6, 0, 0),
                    EnableAuthTokenSlidingExpiration = true,
                    LoginURL = "~/Account/Login",
                    DefaultAuthFwdURL = "~/Account",
                    AcctConfirmURL = "~/Account/Confirm",
                    AcctPswdResetURL = "~/Account/ResetPassword",
                    AcctPswdUpdateURL = "~/Account/UpdatePassword",
                    AcctPswdResetExpiration = new TimeSpan(0, 0, 30, 0),
                    EnableRecaptcha = false,                //CAPTCHA
                    RecaptchaPublicKey = captchaPublicKey,
                    RecaptchaPrivateKey = captchaPrivateKey,
                    AutoConfirmNewAccounts = true,
                    AutoApproveNewAccounts = true,
                    EnableTokenVersioning = false,           //VERSION
                    CookieSameSiteMode = CookieSameSiteModes.Lax,
                    EnablePswdReset = true,
                    EnablePersistentAuthTokens = true,
                    HtmlSanitizerMode = HtmlSanitizerMode.OnWrite | HtmlSanitizerMode.OnRead
                },
                Logging = new CmsConfig_Logging
                {
                    //EnableUserSessionLogging = true,
                    //EnableUserActivityLogging = true,
                    LoggingMode = LoggingMode.SystemEvents,
                    LoggingSource = LoggingSource.UHUB_CMS
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



            CoreFactory.Initialize(cmsConfig);






            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["INIT"] = "INIT";
        }
    }
}
