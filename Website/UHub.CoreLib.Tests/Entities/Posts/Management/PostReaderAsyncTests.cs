using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Posts.DataInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Tools;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;
using System.Collections.Concurrent;

namespace UHub.CoreLib.Entities.Posts.DataInterop.Tests
{
    public partial class PostReaderTests
    {

        [TestMethod()]
        public async Task GetAllPostsAsyncTest()
        {
            TestGlobal.TestInit();

            (await PostReader.GetAllPostsAsync()).ToList();
        }



        [TestMethod()]
        public async Task GetPostAsyncTest()
        {
            TestGlobal.TestInit();


            var postSet = (await PostReader.GetAllPostsAsync()).ToList();

            if (postSet.Count == 0)
            {
                return;
            }

            var id = postSet.First().ID.Value;


            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;

            start = FailoverDateTimeOffset.UtcNow;
            await PostReader.GetPostAsync(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
            start = FailoverDateTimeOffset.UtcNow;
            await PostReader.GetPostAsync(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
        }


        [TestMethod()]
        public async Task GetPostsBySchoolAsyncTest()
        {
            TestGlobal.TestInit();


            var schoolSet = (await SchoolReader.GetAllSchoolsAsync()).ToList();

            if (schoolSet.Count == 0)
            {
                return;
            }

            var schoolId = schoolSet.First().ID.Value;


            var x = await PostReader.GetPostsBySchoolAsync(schoolId);

            Console.WriteLine(x.Count());

        }

        [TestMethod()]
        public async Task GetPostsByClubAsyncTest()
        {
            TestGlobal.TestInit();


            var clubSet = (await SchoolClubReader.GetAllClubsAsync()).ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var clubId = clubSet.First().ID.Value;


            var id = PostReader.GetPostsByClubAsync(clubId);
            Assert.IsNotNull(id);

        }

        [TestMethod]
        public async Task GetPostsBySchoolPageAsyncTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var schoolId = 1;   //CU Boulder



            await PostReader.GetPostsBySchoolPageAsync(schoolId, null);

            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;


            start = FailoverDateTimeOffset.UtcNow;
            (await PostReader.GetPostsBySchoolPageAsync(schoolId, null, null, 5)).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet = (await PostReader.GetPostsBySchoolPageAsync(schoolId, null, 0, 5)).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet2 = (await PostReader.GetPostsBySchoolPageAsync(schoolId, null, 2100, 5)).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 2000: {(end - start).TotalMilliseconds}ms");

        }


        [TestMethod]
        public async Task GetPostsByClubPageAsyncTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var clubSet = (await SchoolClubReader.GetAllClubsAsync()).ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var schoolId = clubSet.First().ID.Value;


            await PostReader.GetPostsByClubPageAsync(schoolId, null);

            await PostReader.GetPostsByClubPageAsync(schoolId, 3, null, 1);


        }


    }
}