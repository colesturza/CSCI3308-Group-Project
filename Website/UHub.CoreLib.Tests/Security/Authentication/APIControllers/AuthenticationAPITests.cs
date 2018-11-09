using System;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Security.Authentication.APIControllers;
using UHub.CoreLib.Extensions;
using System.Web.Http.Controllers;
using System.Net.Http;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Security.Accounts;
using UHub.CoreLib.Entities.Users.DTOs;

namespace UHub.CoreLib.Security.Authentication.APIControllers.Tests
{
    [TestClass]
    public class AuthenticationControllerTests
    {
        [TestMethod]
        public void GetTokenTest()
        {
            TestGlobal.TestInit();

            var controller = TestGlobal.GetStdRequest(new AuthenticationController());
            

            var email = "aual1780@colorado.edu";
            var password = "testtest";


            User_CredentialDTO cred = new User_CredentialDTO()
            {
                Email = email,
                Password = password
            };


            var response = controller.GetToken(cred);
            Assert.IsNotNull(response);

            var result = response as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);


        }

        [TestMethod]
        public void ExtendTokenTest()
        {
            //USE TOKEN AUTH

            TestGlobal.TestInit();

            
            var controller = TestGlobal.GetAuthRequest(new AuthenticationController());

            var response = controller.ExtendToken();
            Assert.IsNotNull(response);

            var result = response as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public void ExtendTokenTest2()
        {
            //USE COOKIE AUTH

            TestGlobal.TestInit();


            var controller = TestGlobal.GetAuthRequest(new AuthenticationController(), true);

            var response = controller.ExtendToken();
            Assert.IsNotNull(response);

            var result = response as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);

        }
    }
}
