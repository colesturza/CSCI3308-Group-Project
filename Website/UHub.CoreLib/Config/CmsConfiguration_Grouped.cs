using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.Hosting;
using System.Data;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;

namespace UHub.CoreLib.Config
{
    /// <summary>
    /// Public CMS configuration module.  Client applications must properly configure the system for use
    /// </summary>
    public sealed class CmsConfiguration_Grouped
    {
        
        public CmsConfig_Instance Instance = null;
        public CmsConfig_DB DB = null;
        public CmsConfig_Storage Storage = null;
        public CmsConfig_Mail Mail = null;
        public CmsConfig_Security Security = null;
        public CmsConfig_Logging Logging = null;
        public CmsConfig_Errors Errors = null;
        public CmsConfig_Caching Caching = null;
        public CmsConfig_API API = null;


        public CmsConfiguration_Grouped()
        {

        }


        /// <summary>
        /// Basic validation for configuration settings
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public bool Validate()
        {
            if(Instance == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_Instance)} cannot be null");
            }
            if(DB == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_DB)} cannot be null");
            }
            if (Storage == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_Storage)} cannot be null");
            }
            if (Mail == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_Mail)} cannot be null");
            }
            if(Security == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_Security)} cannot be null");
            }
            if(Logging == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_Logging)} cannot be null");
            }
            if(Errors == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_Errors)} cannot be null");
            }
            if(Caching == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_Caching)} cannot be null");
            }
            if(API == null)
            {
                throw new ArgumentException($"{nameof(CmsConfig_API)} cannot be null");
            }

            
            //friendly name
            ValidateString(Instance.SiteFriendlyName, nameof(Instance.SiteFriendlyName));
            //base url (only URL that cant be virtual)
            ValidateString(Instance.CmsPublicBaseURL, nameof(Instance.CmsPublicBaseURL));
            if (Instance.CmsPublicBaseURL.RgxIsMatch("[?#]") || !Instance.CmsPublicBaseURL.IsValidURL())
            {
                throw new ArgumentException($"{nameof(Instance.CmsPublicBaseURL)} is not a valid physical URL.");
            }
            //resource url
            ValidateURL(Instance.CmsStaticResourceURL, nameof(Instance.CmsStaticResourceURL));
            ValidateString(Instance.SessionIDCookieName, nameof(Instance.SessionIDCookieName));

            //DB CONNECTIONS
            ValidateSqlConfig(DB.CmsDBConfig, nameof(DB.CmsDBConfig));
            if (!ValidateCmsDB())
            {
                throw new ArgumentException($"{nameof(DB.CmsDBConfig)} is not properly configured.  Please ensure that the TIPC CMS module is installed and your username/password are accurate.");
            }

            //STORAGE CONNECTIONS
            ValidateDirectory(Storage.FileStoreDirectory, nameof(Storage.FileStoreDirectory));
            ValidateDirectory(Storage.ImageStoreDirectory, nameof(Storage.ImageStoreDirectory));
            ValidateDirectory(Storage.TempCacheDirectory, nameof(Storage.TempCacheDirectory));
            if (Logging.LoggingMode == LoggingMode.LocalFile)
            {
                ValidateDirectory(Storage.LogStoreDirectory, nameof(Storage.LogStoreDirectory));
            }



            //SECURITY
            ValidateObject(Security.MaxPswdAge, nameof(Security.MaxPswdAge));
            
            if (Security.MaxPswdAge != null && Security.MaxPswdAge.Ticks < 0)
            {
                throw new ArgumentException($"{nameof(Security.MaxPswdAge)} cannot be less than 0.");
            }
            if (Security.MaxPswdAge != null && Security.MaxPswdAge.Ticks > 0)
            {
                if (!Security.EnablePswdReset)
                {
                    throw new ArgumentException($"{nameof(Security.EnablePswdReset)} must be set if {nameof(Security.MaxPswdAge)} is set.");
                }
            }
            if (Security.ForceHTTPS != Security.ForceSecureCookies)
            {
                throw new ArgumentException($"Security Mismatch - {nameof(Security.ForceHTTPS)} and {nameof(Security.ForceSecureCookies)} must be set to the same value.");
            }
            if (Security.CookieDomain.IsNotEmpty() && !Security.CookieDomain.Split(',').All(dmn => dmn.RgxIsMatch($@"^{RgxPatterns.Cookie.DOMAIN}$", RegexOptions.IgnoreCase)))
            {
                throw new ArgumentException("Invalid cookie domain format.");
            }
            if (Security.EnableRecaptcha)
            {
                ValidateString(Security.RecaptchaPrivateKey, nameof(Security.RecaptchaPrivateKey));
                ValidateString(Security.RecaptchaPublicKey, nameof(Security.RecaptchaPublicKey));
            }
            ValidateObject(Security.MaxAuthTokenLifespan, nameof(Security.MaxAuthTokenLifespan));
            ValidateTimeSpan(Security.AuthTokenTimeout, nameof(Security.AuthTokenTimeout));
            //make sure the token timeout is longer than the token max age
            //mut ensure max lifespan is not infinite
            if (Security.AuthTokenTimeout.Ticks > Security.MaxAuthTokenLifespan.Ticks && Security.MaxAuthTokenLifespan.Ticks > 0)
            {
                throw new ArgumentException(nameof(Security.AuthTokenTimeout) + " must be greater than " + nameof(Security.MaxAuthTokenLifespan));
            }

            //--LOGIN URL
            ValidateURL(Security.LoginURL, nameof(Security.LoginURL));
            //--DEFAULT URL
            ValidateURL(Security.DefaultAuthFwdURL, nameof(Security.DefaultAuthFwdURL));
            //--ACCT CONFIRMATION
            if (!Security.AutoConfirmNewAccounts)
            {
                if (Mail.NoReplyMailConfig == null)
                {
                    throw new ArgumentException($"{nameof(Mail.NoReplyMailConfig)} must be set if AutoConfirmNewAccounts is false.");
                }
                else
                {
                    ValidateEmail(Mail.ContactFormRecipientAddress, nameof(Mail.ContactFormRecipientAddress));
                }

                try
                {
                    if (!Mail.NoReplyMailConfig.Validate())
                    {
                        throw new ArgumentException($"{nameof(Mail.NoReplyMailConfig)} is invalid.");
                    }
                }
                catch (Exception ex)
                {

                    throw new ArgumentException($"{nameof(Mail.NoReplyMailConfig)}: {ex.Message}");
                }

                ValidateURL(Security.AcctConfirmURL, nameof(Security.AcctConfirmURL));
            }

            //PASSWORD UPDATE
            ValidateURL(Security.AcctPswdUpdateURL, nameof(Security.AcctPswdUpdateURL));

            //PASSWORD RESET
            if (Security.EnablePswdReset)
            {
                ValidateURL(Security.AcctPswdResetURL, nameof(Security.AcctPswdResetURL));

                //make sure that MailClient/Pswd reset meta is set if the proxy address is set
                ValidateObject(Mail.NoReplyMailConfig, nameof(Mail.NoReplyMailConfig));
                ValidateTimeSpan(Security.PswdAttemptPeriod, nameof(Security.PswdAttemptPeriod));
                ValidateTimeSpan(Security.PswdLockResetPeriod, nameof(Security.PswdLockResetPeriod));
            }

            if (Security.ForceHTTPS)
            {
                ValidateSecureUrl(Instance.CmsPublicBaseURL, nameof(Instance.CmsPublicBaseURL));
                ValidateSecureUrl(Instance.CmsStaticResourceURL, nameof(Instance.CmsStaticResourceURL));
                ValidateSecureUrl(Security.LoginURL, nameof(Security.LoginURL));
                ValidateSecureUrl(Security.DefaultAuthFwdURL, nameof(Security.DefaultAuthFwdURL));
                ValidateSecureUrl(Security.AcctConfirmURL, nameof(Security.AcctConfirmURL));
                ValidateSecureUrl(Security.AcctPswdResetURL, nameof(Security.AcctPswdResetURL));
                ValidateSecureUrl(Security.AcctPswdUpdateURL, nameof(Security.AcctPswdUpdateURL));
            }

            //CACHING
            if (Caching.EnableDBPageCaching)
            {
                ValidateTimeSpan(Caching.MaxDBCacheAge, nameof(Caching.MaxDBCacheAge));
            }
            if (Caching.EnableIISPageCaching)
            {
                ValidateTimeSpan(Caching.MaxStaticCacheAge, nameof(Caching.MaxStaticCacheAge));
                ValidateTimeSpan(Caching.MaxDynamicCacheAge, nameof(Caching.MaxDynamicCacheAge));
            }

            //API
            //DISABLE ALL API FUNCTIONS IF ROUTES ARE OFF
            if (!API.RegisterAPIRoutes)
            {
                API.EnableAPIFileUploads = false;
                API.EnableAPIAuthService = false;
            }
            if (API.EnableAPIFileUploads)
            {
                ValidateObject(API.MaxFileUploadSize, nameof(API.MaxFileUploadSize));
            }
            else
            {
                API.AllowedFileUploadTypes = new List<FileCategory>();
                API.MaxFileUploadSize = new FileSize(FileSizeUnit.Bytes, 1);
            }

            ValidateObject(API.AllowedFileUploadTypes, nameof(API.AllowedFileUploadTypes));


            return true;
        }


        /// <summary>
        /// Ensure object is not null
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <param name="argName">Name of object variable</param>
        private void ValidateObject(object obj, string argName)
        {
            if (obj == null)
            {
                throw new ArgumentException(argName + " cannot be null.");
            }
        }

        /// <summary>
        /// Ensure that byte is not 0
        /// </summary>
        /// <param name="byteVal">Byte to test</param>
        /// <param name="argName">Name of byte variable</param>
        private void ValidateByte(byte byteVal, string argName)
        {
            if (byteVal == 0)
            {
                throw new ArgumentException(argName + " cannot be 0.");
            }
        }

        /// <summary>
        /// Check that string is not null/empty
        /// </summary>
        /// <param name="str">String to test</param>
        /// <param name="argName">Name of string variable</param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidateString(string str, string argName)
        {
            if (str.IsEmpty())
            {
                throw new ArgumentException(argName + " cannot be null or empty.");
            }
        }

        /// <summary>
        /// Ensure that timespan is not null/empty/0
        /// </summary>
        /// <param name="tSpan">Timespan to test</param>
        /// <param name="argName">Name of timespan variable</param>
        private void ValidateTimeSpan(TimeSpan tSpan, string argName)
        {
            if (tSpan == null || tSpan == TimeSpan.Zero)
            {
                throw new ArgumentException(argName + " cannot be null or empty.");
            }
        }

        /// <summary>
        /// Ensure that string is a valid email
        /// </summary>
        /// <param name="email">Email to test</param>
        /// <param name="argName">Name of email variable</param>
        private void ValidateEmail(string email, string argName)
        {
            if (!email.IsValidEmail())
            {
                throw new ArgumentException(argName + " is not a valid email address.");
            }
        }

        /// <summary>
        /// Attempt to verify that directories are valid. If selected, create missing directories
        /// </summary>
        /// <param name="dir">File directory to test</param>
        /// <param name="argName">Name of directory variable</param>
        private void ValidateDirectory(string dir, string argName)
        {
            ValidateString(dir, argName);


            string tempDir;
            if (dir.RgxIsMatch(@"^\~(\/|\\)") || dir.RgxIsMatch(@"^(\.)+(\/|\\)") || (dir.RgxIsMatch(@"^(\/|\\)") && !dir.RgxIsMatch(@"^(\/\/|\\\\)")))
            {
                try
                {
                    tempDir = HostingEnvironment.MapPath(dir);
                }
                catch
                {
                    throw new ArgumentException(argName + " is not a valid virtual directory");
                }
                if (tempDir.IsEmpty())
                {
                    throw new ArgumentException(argName + " is not a valid virtual directory");
                }
            }
            else
            {
                tempDir = dir.Trim();
            }


            if (!Directory.Exists(tempDir))
            {
                if (Storage.CreateMissingStorageDirectories)
                {
                    try
                    {
                        Directory.CreateDirectory(tempDir);
                    }
                    catch
                    {
                        throw new InvalidOperationException(argName + " cannot be created due to an error");
                    }
                }
                else
                {
                    throw new ArgumentException(argName + " is not a valid directory");
                }
            }
        }

        /// <summary>
        /// Ensure that url is valid, either virtual or physical
        /// </summary>
        /// <param name="url">URL to test</param>
        /// <param name="argName">Name of URL variable</param>
        private void ValidateURL(string url, string argName)
        {
            ValidateString(url, argName);

            var isVirtual = url.RgxIsMatch($@"^{RgxPatterns.Config.INTERNAL_URL}$", RegexOptions.IgnoreCase);
            var isPhysical = !url.RgxIsMatch("[?#]") && url.IsValidURL();

            if (!isVirtual && !isPhysical)
            {
                throw new ArgumentException(argName + " is not a valid URL.  URL must be physical or a root-relative virtual path");
            }
        }

        /// <summary>
        /// Ensure that url uses HTTPS when ForceHTTPS=true
        /// </summary>
        /// <param name="url">Url to test</param>
        /// <param name="argName">Name of the URL variable</param>
        private void ValidateSecureUrl(string url, string argName)
        {
            if (!Security.ForceHTTPS)
            {
                return;
            }

            var lowerUrl = url.ToLower();
            if (!lowerUrl.StartsWith("~/") && !lowerUrl.StartsWith("https://"))
            {
                throw new ArgumentException(argName + " must use the https protocol when ForceHTTPS is enabled.");
            }
        }


        /// <summary>
        /// Ensure that the system can connect to the specified DB with the provided credentials
        /// </summary>
        /// <param name="config">SqlConfig to test</param>
        /// <param name="argName">Name of the config variable</param>
        private void ValidateSqlConfig(SqlConfig config, string argName)
        {
            bool isValid = false;
            try
            {
                isValid = config.ValidateConnection();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(argName + ": " + ex.Message);
            }

            if (!isValid)
            {
                throw new ArgumentException(argName + $" is not valid.  Cannot connect to DB with the specified connection string.");
            }

        }

        /// <summary>
        /// Check DB for test table/view
        /// </summary>
        /// <returns></returns>
        private bool ValidateCmsDB()
        {
            string query =
                @"
                IF (OBJECT_ID('[dbo].[Users]', 'U') IS NOT NULL 
                AND OBJECT_ID('[dbo].[vSchools]', 'V') IS NOT NULL)
                BEGIN
                    select cast(1 as bit)
                END
                else begin
	                select cast(0 as bit)
                end";

            try
            {

                return SqlWorker.ExecScalar<bool>(
                    DB.CmsDBConfig.ToString(),
                    query,
                    (cmd) =>
                    {
                        cmd.CommandType = CommandType.Text;
                    });
            }
            catch
            {
                return false;
            }
        }
    }
}
