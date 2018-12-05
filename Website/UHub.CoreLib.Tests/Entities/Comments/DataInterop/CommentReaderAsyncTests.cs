using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Comments.DataInterop.Test
{
    public partial class CommentReaderTests
    {
        [TestMethod]
        public async Task GetCommentsByPostAsyncTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            var comments = await CommentReader.TryGetCommentsByPostAsync(postID);

            Assert.IsNotNull(comments);
        }


        [TestMethod]
        public async Task GetCommentsByParentAsyncTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            var comments = await CommentReader.TryGetCommentsByParentAsync(postID);

            Assert.IsNotNull(comments);
        }
    }
}
