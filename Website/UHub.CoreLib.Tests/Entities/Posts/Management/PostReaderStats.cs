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
        /*
            Synchronous:
            Iterations: 50
            Total Time: 4081.6312ms
            ItemCount: 10913

            Min: 0.0748247ms
            Max: 0.2132727ms
            Avg: 0.081631628ms
            Median: 0.08446805ms


            -------------------------------------------------------


            Synchronous (threaded):
            Iterations: 50
            Total Time: 3064.1635ms
            ItemCount: 10913

            Min: 0.0769556ms
            Max: 2.6999921ms
            Avg: 0.437710032ms
            Median: 0.1664047ms


            -------------------------------------------------------


            Async:
            Iterations: 50
            Total Time: 4085.8988ms
            ItemCount: 10913

            Min: 0.5083501ms
            Max: 2.732319ms
            Avg: 1.419883718ms
            Median: 2.08540025ms
        */

        //useful metric, but dont include in tests (too time consuming)
        //[TestMethod]
        public async Task GetPostStatTest()
        {
            TestGlobal.TestInit();
            var iterCount = 50;

            {
                var samples1 = new List<double>();
                var itmCount = 0;

                var start_outer = FailoverDateTimeOffset.UtcNow;


                for (int i = 0; i < iterCount; i++)
                {
                    var start = FailoverDateTimeOffset.UtcNow;

                    var set = SqlWorker.ExecBasicQuery<Post>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Posts_GetAll]",
                    (cmd) => { })
                    .ToList();

                    var end = FailoverDateTimeOffset.UtcNow;


                    itmCount = set.Count;
                    double sample = (end - start).TotalSeconds;
                    samples1.Add(sample);
                }

                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;


                Console.WriteLine($"Synchronous:");
                Console.WriteLine($"Iterations: {iterCount}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
            }
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
                    (cmd) => { })
                    .ToList();

                    var end = FailoverDateTimeOffset.UtcNow;


                    itmCount = set.Count;
                    double sample = (end - start).TotalSeconds;
                    samples1.Add(sample);
                });

                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;


                Console.WriteLine($"Synchronous (threaded):");
                Console.WriteLine($"Iterations: {iterCount}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
            }


            {
                var samples1 = new ConcurrentBag<double>();
                var itmCount = 0;


                async Task worker()
                {
                    var start = FailoverDateTimeOffset.UtcNow;

                    var set = await SqlWorker.ExecBasicQueryAsync<Post>(
                        CoreFactory.Singleton.Properties.CmsDBConfig,
                        "[dbo].[Posts_GetAll]",
                        (cmd) => { });

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
                await Task.WhenAll(taskSet);


                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;


                Console.WriteLine($"Async:");
                Console.WriteLine($"Iterations: {iterCount}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");

            }

        }

    }
}