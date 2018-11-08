using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Security.Authentication
{
    internal static partial class TokenManager
    {


        internal static TokenValidator GetValidator(AuthenticationToken token)
        {
            TokenValidator validator = null;

            try
            {
                validator = DataInterop.SqlWorker.ExecBasicQuery<TokenValidator>
                (
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetTokenValidator]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@IssueDate", SqlDbType.DateTimeOffset).Value = token.IssueDate.UtcDateTime;
                        cmd.Parameters.Add("@TokenID", SqlDbType.NVarChar).Value = token.TokenID;
                    },
                    (reader) =>
                    {
                        return reader.ToCustomDBType<TokenValidator>();
                    }
                ).SingleOrDefault();
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                return null;
            }


            return validator;
        }

        internal static void SaveTokenValidatorToDB(AuthenticationToken token)
        {
            DateTimeOffset issue = token.IssueDate.UtcDateTime;
            DateTimeOffset expiration;

            var maxLifeSpan = CoreFactory.Singleton.Properties.MaxAuthTokenLifespan;

            if (maxLifeSpan.Ticks == 0)
            {
                expiration = DateTimeOffset.MaxValue;
            }
            else
            {
                expiration = issue.Add(maxLifeSpan).UtcDateTime;
            }


            DataInterop.SqlWorker.ExecNonQuery
            (
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_CreateTokenValidator]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@IssueDate", SqlDbType.DateTimeOffset).Value = issue;
                    cmd.Parameters.Add("@MaxExpirationDate", SqlDbType.DateTimeOffset).Value = expiration;
                    cmd.Parameters.Add("@TokenID", SqlDbType.NVarChar).Value = token.TokenID;
                    cmd.Parameters.Add("@IsPersistent", SqlDbType.Bit).Value = token.IsPersistent;
                    cmd.Parameters.Add("@TokenHash", SqlDbType.NVarChar).Value = token.GetTokenHash();
                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = token.SessionID ?? "";
                    cmd.Parameters.Add("@RequestID", SqlDbType.NVarChar).Value = "";    //NOT USED
                }
            );
        }

        internal static void RevokeTokenValidator(AuthenticationToken token)
        {
            DataInterop.SqlWorker.ExecNonQuery
            (
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_RevokeTokenValidator]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@IssueDate", SqlDbType.DateTimeOffset).Value = token.IssueDate;
                    cmd.Parameters.Add("@TokenID", SqlDbType.NVarChar).Value = token.TokenID;
                }
            );
        }


        internal static void PurgeExpiredTokenValidators()
        {
            DataInterop.SqlWorker.ExecNonQuery
           (
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[Users_PurgeExpiredTokenValidators]",
               (cmd) => { }
           );
        }

        /// <summary>
        /// Checks token against DB to ensure that the given token was issued by the security system and has not been revoked.  Also checks Date, SessionID, and RequestToken
        /// </summary>
        /// <param name="token"></param>
        /// <param name="SessionID"></param>
        /// <param name="tokenStatus"></param>
        /// <returns></returns>
        internal static bool IsTokenValid(AuthenticationToken token, string SessionID, out TokenValidationStatus tokenStatus)
        {
            var now = FailoverDateTimeOffset.UtcNow;

            TokenValidator validator = GetValidator(token);

            if (validator == null)
            {
                tokenStatus = TokenValidationStatus.TokenValidatorNotFound;
                return false;
            }
            //check revocation
            if (!validator.IsValid)
            {
                tokenStatus = TokenValidationStatus.TokenValidatorRevoked;
                return false;
            }
            //check for date cohesion
            if (token.IssueDate != validator.IssueDate)
            {
                tokenStatus = TokenValidationStatus.TokenValidatorInvalid;
                return false;
            }
            //ensure that the issue date has already passed
            if (token.IssueDate.UtcDateTime > now)
            {
                tokenStatus = TokenValidationStatus.TokenInvalid;
                return false;
            }
            //ensure that the expiration date is in the future
            //ensure that the MaxAge is in the future
            if (token.ExpirationDate.UtcDateTime < now || validator.MaxExpirationDate < now)
            {
                tokenStatus = TokenValidationStatus.TokenExpired;
                return false;
            }
            //ensure token/validator cohesion
            //hash
            //sessionID
            //persistence
            //requestToken
            if (token.GetTokenHash() != validator.TokenHash || token.SessionID != validator.SessionID || token.IsPersistent != validator.IsPersistent)
            {
                tokenStatus = TokenValidationStatus.TokenValidatorMismatch;
                return false;
            }
            //ensure that the token is still pinned to the proper client session
            //only important is token is not persistent
            if (!token.IsPersistent && token.SessionID != SessionID)
            {
                tokenStatus = TokenValidationStatus.TokenSessionMismatch;
                return false;
            }


            tokenStatus = TokenValidationStatus.Success;
            return true;
        }


    }
}
