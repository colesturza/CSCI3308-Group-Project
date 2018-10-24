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
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Tests.Security.Authentication.APIControllers.Tests
{
    [TestClass]
    public class AuthenticationControllerTests
    {
        [TestMethod]
        public void GetTokenTest()
        {
            TestGlobal.TestInit();



            var controller = new AuthenticationController();
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://dev.u-hub.life");

            controllerContext.Request = request;

            controller.ControllerContext = controllerContext;
            

            var email = "aual1780@colorado.edu";
            var password = "testtest";


            var response = controller.GetToken(email, password);
            Assert.IsNotNull(response);

            var result = response as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);


            Console.WriteLine(result.Content);


        }
    }
}
