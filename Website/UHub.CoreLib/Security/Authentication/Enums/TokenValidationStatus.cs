using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Authentication
{
    /// <summary>
    /// Status returned when token is validated
    /// </summary>
    public enum TokenValidationStatus
    {
        /// <summary>
        /// Token validation succeeded
        /// </summary>
        Success,
        /// <summary>
        /// Token was processed, but returned an anonymous user
        /// </summary>
        AnonUser,
        /// <summary>
        /// Client token not found
        /// </summary>
        TokenNotFound,
        /// <summary>
        /// Client token could not be validated
        /// </summary>
        TokenInvalid,
        /// <summary>
        /// Client token could not be decrypted
        /// </summary>
        TokenAESFailure,
        /// <summary>
        /// Client token has expired
        /// </summary>
        TokenExpired,
        /// <summary>
        /// Client token does not have current expected version
        /// </summary>
        TokenVersionMismatch,
        /// <summary>
        /// Client token has been revoked
        /// </summary>
        TokenValidatorRevoked,
        /// <summary>
        /// Client token does not have a DB validator
        /// </summary>
        TokenValidatorNotFound,
        /// <summary>
        /// Client token does not match expected validator
        /// </summary>
        TokenValidatorMismatch,
        /// <summary>
        /// Token validator could be validated
        /// </summary>
        TokenValidatorInvalid,
        /// <summary>
        /// Client token does not match current client session
        /// </summary>
        TokenSessionMismatch,
        /// <summary>
        /// The user encapsulated by the token could not be validated
        /// </summary>
        TokenUserError,
        /// <summary>
        /// The user encapsulated by the token is not allowed to login
        /// </summary>
        TokenUserForbidden,
        /// <summary>
        /// The user encapsulated by the token is not confirmed
        /// </summary>
        TokenUserNotConfirmed,
        /// <summary>
        /// The user encapsulated by the token is not approved
        /// </summary>
        TokenUserNotApproved,
        /// <summary>
        /// The user encapsulated by the token has been disabled
        /// </summary>
        TokenUserDisabled
    }
}
