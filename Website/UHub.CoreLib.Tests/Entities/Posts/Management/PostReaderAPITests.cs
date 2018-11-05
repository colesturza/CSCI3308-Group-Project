﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Posts.APIControllers;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.Posts.Management.Tests
{
    [TestClass]
    public class PostReaderAPITests
    {
        [TestMethod]
        public void GetPostCountBySchoolTest()
        {
            TestGlobal.TestInit();

            var controller = TestGlobal.GetAuthRequest(new PostController(), true);

            var response = controller.GetPostCountBySchool();
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);
            Console.WriteLine(result.Content);

        }


        [TestMethod]
        public void GetPageCountBySchoolTest()
        {
            TestGlobal.TestInit();

            var controller = TestGlobal.GetAuthRequest(new PostController(), true);

            var response = controller.GetPageCountBySchool();
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public void GetPageBySchoolTest()
        {
            TestGlobal.TestInit();

            var controller = TestGlobal.GetAuthRequest(new PostController(), true);


            dynamic response = controller.GetPageBySchool();
            Assert.IsNotNull(response);

            dynamic result = response.Content;
            Assert.IsNotNull(result);

        }



        [TestMethod]
        public void GetPostCountByClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.GetClubsByDomain("@colorado.edu").ToList();
            var clubID = clubSet.FirstOrDefault()?.ID;

            if (clubID == null)
            {
                return;
            }


            var controller = TestGlobal.GetAuthRequest(new PostController(), true);

            var response = controller.GetPostCountByClub(clubID.Value);
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public void GetPageCountByClubTest()
        {
            TestGlobal.TestInit();

            var clubSet = SchoolClubReader.GetClubsByDomain("@colorado.edu").ToList();
            var clubID = clubSet.FirstOrDefault()?.ID;

            if (clubID == null)
            {
                return;
            }

            var controller = TestGlobal.GetAuthRequest(new PostController(), true);

            var response = controller.GetPageCountByClub(clubID.Value);
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public void GetPageByClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.GetClubsByDomain("@colorado.edu").ToList();
            var clubID = clubSet.FirstOrDefault()?.ID;

            if (clubID == null)
            {
                return;
            }

            var controller = TestGlobal.GetAuthRequest(new PostController(), true);

            dynamic response = controller.GetPageByClub(clubID.Value);
            Assert.IsNotNull(response);

            dynamic result = response.Content;
            Assert.IsNotNull(result);

        }

    }
}
