using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Security.Authentication.Interfaces
{
    /// <summary>
    /// Authentication manager interface to control method exposure
    /// </summary>
    public interface IAuthenticationManager
    {
        /// <summary>
        /// Validate user credentials then set authentication token via cookie
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="ResultHandler">Result handler to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        void TrySetClientAuthToken(
            string UserEmail, 
            string UserPassword, 
            bool IsPersistent,
            Action<AuthResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null);

        /// <summary>
        /// Validate user credentials then return encrypted authentication token
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="ResultHandler">Result handler to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        string TryGetClientAuthToken(
            string UserEmail, 
            string UserPassword, 
            bool IsPersistent,
            Action<AuthResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null);

        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        string TrySlideAuthTokenExpiration(string token);
        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        string TrySlideAuthTokenExpiration(string token, out TokenValidationStatus TokenStatus);

        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="ResultHandler">Result handler to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        bool TryAuthenticateUser(
            string UserEmail, 
            string UserPassword,
            Action<AuthResultCode> ResultHandler = null,
            Action<Guid> GeneralFailHandler = null);

        /// <summary>
        /// Forward user to originally requested page (if set) or default auth page
        /// </summary>
        void AuthUserPageForward();

        /// <summary>
        /// Set the user for the current request using an Auth Token.  If the token is invalid, then the user will be set to ANON privs.
        /// This method is provided so that restful API requests without an auth cookie can be authenticated throughout the CMS pipeline
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="tokenStatus">Returns token validation status</param>
        /// <returns>Status flag</returns>
        bool TrySetRequestUser(string tokenStr, out TokenValidationStatus tokenStatus);

        /// <summary>
        /// Ensure that auth token is valid and user is logged in
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="CmsUser">User encapsulated by auth token (if valid)</param>
        /// <param name="tokenStatus">Returns token validation status</param>
        /// <returns></returns>
        bool ValidateAuthToken(string tokenStr, out User CmsUser, out TokenValidationStatus tokenStatus);

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// <returns></returns>
        bool IsUserLoggedIn();

        /// <summary>
        /// Determine if there is a user logged in
        /// Uses the auth cookie and various expiration timers
        /// Returns the authenticated user or a reference to Anon instance
        /// </summary>
        /// <returns></returns>
        bool IsUserLoggedIn(out User CmsUser);

        /// <summary>
        /// Determine if there is a user logged in
        /// Uses the auth cookie and various expiration timers
        /// Returns the authenticated user or a reference to Anon instance
        /// </summary>
        /// <returns></returns>
        bool IsUserLoggedIn(out User CmsUser, out TokenValidationStatus tokenStatus);

        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        User GetCurrentUser();

        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        User GetCurrentUser(out TokenValidationStatus tokenStatus);



        /// <summary>
        /// Log out of all devices by changing user version
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ExcludeCurrent"></param>
        void LogoutOfAllDevices(long UserID, bool ExcludeCurrent = false);

        /// <summary>
        /// Log out of all devices by changing user version
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="ExcludeCurrent"></param>
        void LogoutOfAllDevices(string Email, bool ExcludeCurrent = false);

        /// <summary>
        /// Log out of all devices by changing user version
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        /// <param name="ExcludeCurrent"></param>
        void LogoutOfAllDevices(string Username, string Domain, bool ExcludeCurrent = false);

        /// <summary>
        /// Remove persistent cookies from request/response
        /// </summary>
        void LogOut();

    }
}
