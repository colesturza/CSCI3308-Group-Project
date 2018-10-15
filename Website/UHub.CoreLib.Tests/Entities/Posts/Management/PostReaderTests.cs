using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Posts.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Posts.Management.Tests
{
    [TestClass()]
    public class PostReaderTests
    {
        [TestMethod()]
        public void GetPostsTest()
        {
            TestGlobal.TestInit();


            PostReader.GetPosts().ToList();
        }
    }
}