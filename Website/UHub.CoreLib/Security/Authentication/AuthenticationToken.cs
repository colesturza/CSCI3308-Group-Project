﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysSec = System.Web.Security;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Tools;


namespace UHub.CoreLib.Security.Authentication
{
    internal sealed class AuthenticationToken
    {
        private const string purpose = "AuthTokenEncryption";

        public bool IsPersistent { get; private set; }
        public string TokenID { get; private set; }
        private string TokenSalt { get; set; }
        public DateTimeOffset IssueDate { get; private set; }
        public DateTimeOffset ExpirationDate { get; private set; }
        public long UserID { get; private set; }
        public int SystemVersion { get; private set; }
        public string UserVersion { get; private set; }
        public string RequestID { get; private set; }
        public string SessionID { get; private set; }

        public AuthenticationToken(bool IsPersistent, DateTimeOffset IssueDate, DateTimeOffset ExpirationDate, long UserID, int SystemVersion, string UserVersion, string SessionID)
        {
            Random rnd = new Random();

            //get rnd ID in case multiple treads hit at same time
            //1296 --> 100
            //46655 --> ZZZ
            //46656 --> 1000
            //1679615 --> ZZZZ
            this.TokenID = Base36.IntToString(rnd.Next(1296, 46655)).ToLower();


            string salt = SysSec.Membership.GeneratePassword(10, 0);
            salt = salt.RgxReplace(@"[^a-zA-Z0-9]", Base36.IntToString(rnd.Next(35)));
            this.TokenSalt = salt;

            this.IsPersistent = IsPersistent;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.UserID = UserID;
            this.SystemVersion = SystemVersion;
            this.UserVersion = UserVersion;
            this.SessionID = SessionID ?? "";

            //TODO: fully implement request token or get rid of it
            //represents an extra layer of security, similar to SessionID
            string tkn = SysSec.Membership.GeneratePassword(5, 0);
            tkn = tkn.RgxReplace(@"[^a-zA-Z0-9]", Base36.IntToString(rnd.Next(35)));
            this.RequestID = tkn;
        }

        private AuthenticationToken(string TokenID, string TokenSalt, bool IsPersistent, DateTimeOffset IssueDate, DateTimeOffset ExpirationDate, long UserID, int SystemVersion, string UserVersion, string SessionID, string RequestToken)
        {
            this.TokenID = TokenID;
            this.TokenSalt = TokenSalt;
            this.IsPersistent = IsPersistent;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.UserID = UserID;
            this.SystemVersion = SystemVersion;
            this.UserVersion = UserVersion;
            this.SessionID = SessionID ?? "";
            this.RequestID = RequestToken;
        }

        internal void SlideExpiration(TimeSpan offset)
        {
            ExpirationDate = ExpirationDate.Add(offset);
        }
        internal void SetExpiration(DateTimeOffset newDate)
        {
            ExpirationDate = newDate;
        }


        internal string Serialize()
        {
            StringBuilder data = new StringBuilder();

            data.Append(TokenID);
            data.Append("|");
            data.Append(TokenSalt);
            data.Append("|");
            data.Append(IsPersistent ? "1" : "0");
            data.Append("|");
            data.Append(Base36.LongToString(IssueDate.UtcTicks));
            data.Append("|");
            data.Append(Base36.LongToString(ExpirationDate.UtcTicks));
            data.Append("|");
            data.Append(Base36.LongToString(UserID));
            data.Append("|");
            data.Append(SystemVersion);
            data.Append("|");
            data.Append(UserVersion);
            data.Append("|");
            data.Append(SessionID);
            data.Append("|");
            data.Append(RequestID);

            return data.ToString();
        }

        /// <summary>
        /// Get string from AuthToken for use in cookies 
        /// <para/> Strips out data that should not be send to client
        /// </summary>
        /// <returns></returns>
        public string Encrypt()
        {

            return Serialize().Encrypt(purpose);
        }

        /// <summary>
        /// Get AuthToken from string
        /// <para/> Retrieves data that should not be sent to client
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AuthenticationToken Decrypt(string data)
        {
            try
            {
                var plainData = data.Decrypt(purpose);

                var parts = plainData.Split('|');

                if (parts.Count() != 10)
                {
                    throw new Exception("Invalid token data");
                }

                //ID
                string tokenId = parts[0];
                //SALT
                string tokenSalt = parts[1];
                //PERSIST
                bool persist = (parts[2] == "1");
                //ISSUE DATE
                var issueTicks = Base36.StringToLong(parts[3]);
                DateTimeOffset issueDate = new DateTimeOffset(issueTicks, new TimeSpan(0));
                //EXPIRE DATE
                var expireTicks = Base36.StringToLong(parts[4]);
                DateTimeOffset expirationDate = new DateTimeOffset(expireTicks, new TimeSpan(0));
                //USER ID
                long userID = Base36.StringToLong(parts[5]);
                //SYSTEM VERSION
                int systemV = int.Parse(parts[6]);
                //USER VERSION
                string userV = parts[7];
                //SESSION ID
                string sessionId = parts[8];
                //REQUEST TKN
                string requestTkn = parts[9];

                return new AuthenticationToken(tokenId, tokenSalt, persist, issueDate, expirationDate, userID, systemV, userV, sessionId, requestTkn);
            }
            catch
            {
                throw;
            }
        }


        public string GetTokenHash(HashType HashType = HashType.HMACSHA256)
        {
            //hash all fields except Expiration date
            //this allows the client to slide the token expiration without affecting the hash
            //to maintain security, the max token lifespan is stored in the DB validator

            StringBuilder data = new StringBuilder();

            data.Append(TokenID);
            data.Append("|");
            data.Append(TokenSalt);
            data.Append("|");
            data.Append(IsPersistent);
            data.Append("|");
            data.Append(IssueDate.UtcTicks);
            data.Append("|");
            data.Append(UserID);
            data.Append("|");
            data.Append(SystemVersion);
            data.Append("|");
            data.Append(UserVersion);
            data.Append("|");
            data.Append(SessionID);
            data.Append("|");
            data.Append(RequestID);


            return data.ToString().GetHash(HashType);
        }


    }
}