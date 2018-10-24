using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Comments.Management;
using UHub.CoreLib.Entities.Posts.Management;

namespace UHub.CoreLib.Tests.Entities.Comments.Management.Tests
{
    [TestClass]
    public class CommentReaderTests
    {
        [TestMethod]
        public void GetCommentsByPostTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            CommentReader.GetCommentsByPost(postID);

        }


        [TestMethod]
        public void GetCommentsByParentTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            CommentReader.GetCommentsByParent(postID);

        }
    }
}
