using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Posts.APIControllers;
using UHub.CoreLib.Entities.Posts.DTOs;

namespace UHub.CoreLib.Tests.Entities.Posts.Management.Tests
{
    [TestClass]
    public class PostReadAPITests
    {
        [TestMethod]
        public void GetPageBySchoolTest()
        {
            TestGlobal.TestInit();

            var controller = TestGlobal.GetAuthRequest(new PostController(), true);

            var response = controller.GetPageBySchool();
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<IEnumerable<Post_R_PublicDTO>>;
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void GetPageBySchoolTest2()
        {
            TestGlobal.TestInit();

            try
            {
                Type sct = typeof(PostController);
                MethodInfo mInfo = sct.GetMethod("GetPageBySchool");
                var match = mInfo.GetCustomAttributes(typeof(ApiAuthControlAttribute), false);
                Assert.AreEqual(match.Length, 1);


                var controller = TestGlobal.GetStdRequest(new PostController());
                var response = controller.GetPageBySchool();
                Assert.IsNotNull(response);


                var result = response as OkNegotiatedContentResult<IEnumerable<Post_R_PublicDTO>>;

                //flow should not make it this far
                Assert.Fail();

            }
            catch
            {

            }
        }


    }
}
