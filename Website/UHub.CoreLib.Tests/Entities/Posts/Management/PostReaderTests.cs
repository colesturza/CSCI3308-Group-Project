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
using System.Collections.Concurrent;

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


            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;

            start = FailoverDateTimeOffset.UtcNow;
            PostReader.GetPost(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
            start = FailoverDateTimeOffset.UtcNow;
            PostReader.GetPost(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");


            Console.WriteLine();


            start = FailoverDateTimeOffset.UtcNow;
            PostReader.GetPostAsync(id).Wait();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
            start = FailoverDateTimeOffset.UtcNow;
            PostReader.GetPostAsync(id).Wait();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
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


            var x = PostReader.GetPostsBySchool(schoolId);

            Console.WriteLine(x.Count());

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


            var schoolId = 1;   //CU Boulder



            PostReader.GetPostsBySchoolPage(schoolId, null, out _);

            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;


            start = FailoverDateTimeOffset.UtcNow;
            PostReader.GetPostsBySchoolPage(schoolId, null, null, 5).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet = PostReader.GetPostsBySchoolPage(schoolId, null, 0, 5).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet2 = PostReader.GetPostsBySchoolPage(schoolId, null, 2100, 5).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 2000: {(end - start).TotalMilliseconds}ms");

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
        public async Task GetPostStatTest()
        {
            TestGlobal.TestInit();
            var iterCount = 50;

            {
                var samples1 = new List<double>();
                var itmCount = 0;

                var start_outer = FailoverDateTimeOffset.UtcNow;


                Parallel.For(0, iterCount, (x) =>
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
                });

                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;

                Console.WriteLine($"Synchronous:");
                Console.WriteLine($"Iterations: {iterCount}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");
            }
            Console.WriteLine("-------------------------------------------------------");
            {
                var samples1 = new ConcurrentBag<double>();
                var itmCount = 0;


                async Task worker()
                {
                    var start = FailoverDateTimeOffset.UtcNow;

                    var set = await SqlWorker.ExecBasicQueryAsync<Post>(
                        CoreFactory.Singleton.Properties.CmsDBConfig,
                        "[dbo].[Posts_GetAll]",
                        (cmd) => { },
                        (reader) =>
                        {
                            return reader.ToCustomDBType<Post>();
                        });

                    var end = FailoverDateTimeOffset.UtcNow;

                    itmCount = set.Count();
                    double sample = (end - start).TotalSeconds;
                    samples1.Add(sample);
                }


                var taskSet = new List<Task>();
                var start_outer = FailoverDateTimeOffset.UtcNow;
                for (int i = 0; i < iterCount; i++)
                {
                    taskSet.Add(worker());
                }
                await Task.WhenAll(taskSet.ToArray());


                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;

                Console.WriteLine($"Async:");
                Console.WriteLine($"Iterations: {iterCount}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");
            }
        }

    }
}