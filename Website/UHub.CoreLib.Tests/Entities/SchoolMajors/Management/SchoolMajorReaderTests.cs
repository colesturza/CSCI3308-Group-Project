using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.SchoolMajors.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.SchoolMajors.Management.Tests
{
    [TestClass()]
    public class SchoolMajorReaderTests
    {
        [TestMethod()]
        public void GetSchoolMajorsTest()
        {
            TestGlobal.TestInit();


            SchoolMajorReader.GetSchoolMajors().ToList();

        }
    }
}