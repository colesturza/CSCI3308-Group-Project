using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Posts;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.Management;

namespace UHub.CoreLib.Tests.Entities.Posts.Management.Tests
{
    [TestClass]
    public class PostWriterTests
    {
        [TestMethod]
        public void TryCreatePostTest()
        {
            var cub = 1;    //CU Boulder ID


            TestGlobal.TestInit();

            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST " + DateTimeOffset.UtcNow,
                Content = "A1F27E0D-35E5-4323-9BAF-6C86A6E2A394",
                ParentID = cub,
                CanComment = true,
                IsPublic = true

            };

            var post = testPost.ToInternal<Post>();


            var postId = PostWriter.TryCreatePost(post);


            if (postId == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postId);

        }
    }
}
