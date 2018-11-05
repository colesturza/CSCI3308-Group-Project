using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Posts.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Tools;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Posts.Management.Tests
{
    [TestClass()]
    public class PostReaderTests
    {
        [TestMethod()]
        public void GetAllPostsTest()
        {
            TestGlobal.TestInit();

            PostReader.GetAllPosts().ToList();
        }

        [TestMethod()]
        public void GetAllPostsDTOTest()
        {
            TestGlobal.TestInit();

            var postSet = PostReader.GetAllPosts().ToList();


            var dtoSet = postSet.Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();


            postSet = dtoSet.Select(x => x.ToInternal<Post>()).ToList();

        }


        [TestMethod()]
        public void GetPostTest()
        {
            TestGlobal.TestInit();


            var postSet = PostReader.GetAllPosts().ToList();

            if (postSet.Count == 0)
            {
                return;
            }

            var id = postSet.First().ID.Value;

            PostReader.GetPost(id);

        }


        [TestMethod()]
        public void GetPostsBySchoolTest()
        {
            TestGlobal.TestInit();


            var schoolSet = SchoolReader.GetAllSchools().ToList();

            if (schoolSet.Count == 0)
            {
                return;
            }

            var schoolId = schoolSet.First().ID.Value;


            PostReader.GetPostsBySchool(schoolId);

        }

        [TestMethod()]
        public void GetPostsByClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.GetAllClubs().ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var clubId = clubSet.First().ID.Value;


            var id = PostReader.GetPostsByClub(clubId);
            Assert.IsNotNull(id);

        }

        [TestMethod]
        public void GetPostsBySchoolPageTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var schoolSet = SchoolReader.GetAllSchools().ToList();

            if (schoolSet.Count == 0)
            {
                return;
            }

            var schoolId = schoolSet.First().ID.Value;


            PostReader.GetPostsBySchoolPage(schoolId, null, out _);

            PostReader.GetPostsBySchoolPage(schoolId, 3, null, 1);

        }


        [TestMethod]
        public void GetPostsByClubPageTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.GetAllClubs().ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var schoolId = clubSet.First().ID.Value;


            PostReader.GetPostsByClubPage(schoolId, null, out _);

            PostReader.GetPostsByClubPage(schoolId, 3, null, 1);


        }



        [TestMethod]
        public void GetPostStatTest()
        {
            TestGlobal.TestInit();

            var iterCount = 50;
            var samples1 = new List<double>();
            var itmCount = 0;

            var start_outer = FailoverDateTimeOffset.UtcNow;
            for (int i = 0; i < iterCount; i++)
            {
                var start = FailoverDateTimeOffset.UtcNow;

                var set = SqlWorker.ExecBasicQuery<Post>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Posts_GetAll]",
                    (cmd) => { },
                    (reader) =>
                    {
                        return reader.ToCustomDBType<Post>();
                    }).ToList();

                var end = FailoverDateTimeOffset.UtcNow;

                itmCount = set.Count;
                double sample = (end - start).TotalSeconds;
                samples1.Add(sample);
            }
            var end_outer = FailoverDateTimeOffset.UtcNow;
            var totalTime = (end_outer - start_outer).TotalMilliseconds;

            Console.WriteLine($"Total Time: {totalTime}ms");
            Console.WriteLine($"ItemCount: {itmCount}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Iterations: {iterCount}");
            Console.WriteLine($"Min: {samples1.Min()}ms");
            Console.WriteLine($"Max: {samples1.Max()}ms");
            Console.WriteLine($"Avg: {samples1.Average()}ms");
            Console.WriteLine($"Median: {samples1.Median()}ms");
        }

    }
}