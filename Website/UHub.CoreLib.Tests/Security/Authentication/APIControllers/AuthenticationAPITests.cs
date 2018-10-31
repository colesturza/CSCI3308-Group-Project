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


            var response = controller.GetToken(email, password);
            Assert.IsNotNull(response);

            var result = response as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);


        }

        [TestMethod]
        public void ExtendTokenTest()
        {
            //USE TOKEN AUTH

            TestGlobal.TestInit();


            var controller = TestGlobal.GetAuthRequest(new AuthenticationController(), true);

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
