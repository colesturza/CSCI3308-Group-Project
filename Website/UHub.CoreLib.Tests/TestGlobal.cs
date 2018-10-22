using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Http;
using UHub.CoreLib.Config;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.SmtpInterop;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;

namespace UHub.CoreLib.Tests
{
    public class TestGlobal
    {

        public static void TestInit()
        {
            if (!CoreFactory.IsInitialized())
            {

                var configPath = @"\\thelairserv4\FileHosting\IISData\UHub\_configs\Test_Raw";
                string getVar(string name)
                {
                    return File.ReadAllText(Path.Combine(configPath, name + ".txt"));
                }

                //DB
                var dbConn = getVar("DB_Conn");

                //MAIL
                var ContactFormRecipient = getVar("ContactFormRecipient");
                var mailFromAddr = getVar("MailFromAddress");
                var mailFromName = getVar("MailFromName");
                var mailHost = getVar("MailHost");
                var mailPort = int.Parse(getVar("MailPort"));
                var mailUsername = getVar("MailUsername");
                var mailPswd = getVar("MailPassword");


                //FILES
                var fileStoreDir = getVar("FileStoreDir");
                var imgStoreDir = getVar("ImageStoreDir");
                var tempCacheDir = getVar("TempCacheDir");
                var logStoreDir = getVar("LogStoreDir");


                //CAPTCHA
                var captchaPublicKey = getVar("RecaptchaPublicKey");
                var captchaPrivateKey = getVar("RecaptchaPrivateKey");


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
                        LoginURL = "https://u-hub.life/Account/Login",
                        DefaultAuthFwdURL = "https://u-hub.life/Account",
                        AcctConfirmURL = "https://u-hub.life/Account/Confirm",
                        AcctPswdResetURL = "https://u-hub.life/Account/ResetPassword",
                        AcctPswdUpdateURL = "https://u-hub.life/Account/UpdatePassword",
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
                        LoggingMode = LoggingMode.LocalFile,
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

            }

        }


    }
}
