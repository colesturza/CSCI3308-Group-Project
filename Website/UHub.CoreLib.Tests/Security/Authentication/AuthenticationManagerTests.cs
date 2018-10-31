using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Security.Authentication.APIControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.Security.Authentication.Tests
{
    [TestClass()]
    public class AuthenticationManagerTests
    {
        [TestMethod()]
        public void GetTokenTest()
        {
            TestGlobal.TestInit();


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            string status = "Login Failed";

            //LOGIN INVALID
            var email = "aual1780@colorado.edu";
            var password = "TEST";
            var persistent = false;


            var token = CoreFactory.Singleton.Auth.TryGetClientAuthToken(
                    email,
                    password,
                    persistent,
                    ResultHandler: (authCode) =>
                    {
                        if (enableDetail)
                        {
                            switch (authCode)
                            {
                                case AuthResultCode.EmailEmpty: { status = "Email Empty"; break; }
                                case AuthResultCode.EmailInvalid: { status = "Email Invalid"; break; }
                                case AuthResultCode.PswdEmpty: { status = "Password Empty"; break; }
                                case AuthResultCode.UserInvalid: { status = "Account Invalid"; break; }
                                case AuthResultCode.UserLocked: { status = "Account Locked"; break; }
                                case AuthResultCode.PendingApproval: { status = "Account Pending Approval"; break; }
                                case AuthResultCode.PendingConfirmation: { status = "Account Pending Confirmation"; break; }
                                case AuthResultCode.UserDisabled: { status = "Account Disabled"; break; }
                                case AuthResultCode.PswdExpired: { status = "Password Expired"; break; }
                                case AuthResultCode.CredentialsInvalid: { status = "Credentials Invalid"; break; }
                                case AuthResultCode.Success: { status = "Unknown Error"; break; }
                            }
                        }

                    },
                    GeneralFailHandler: (code) =>
                    {
                        Assert.Fail();
                        if (enableFailCode)
                        {
                            status = code.ToString();
                        }
                    });


            Console.WriteLine(status);
            Assert.AreEqual("Credentials Invalid", status);
        }
    }
}