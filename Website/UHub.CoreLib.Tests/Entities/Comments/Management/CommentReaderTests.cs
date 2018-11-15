using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Comments.Management.Tests
{
    [TestClass]
    public partial class CommentReaderTests
    {
        [TestMethod]
        public void GetCommentsByPostTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            var comments = CommentReader.GetCommentsByPost(postID);

            Assert.IsNotNull(comments);
        }


        [TestMethod]
        public void GetCommentsByParentTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            var comments = CommentReader.GetCommentsByParent(postID);

            Assert.IsNotNull(comments);
        }
    }
}
