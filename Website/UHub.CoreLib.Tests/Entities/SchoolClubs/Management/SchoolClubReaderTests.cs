using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.SchoolClubs.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.SchoolClubs.Management.Tests
{
    [TestClass()]
    public class SchoolClubReaderTests
    {
        [TestMethod()]
        public void GetSchoolClubsTest()
        {
            TestGlobal.TestInit();


            SchoolClubReader.GetSchoolClubs().ToList();
        }
    }
}