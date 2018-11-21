using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.SchoolMajors.DataInterop.Tests
{
    [TestClass()]
    public class SchoolMajorReaderTests
    {
        [TestMethod()]
        public void GetAllMajorsTest()
        {
            TestGlobal.TestInit();


            SchoolMajorReader.GetAllMajors().ToList();

        }


        [TestMethod()]
        public void GetMajorTest()
        {
            TestGlobal.TestInit();


            var majorSet = SchoolMajorReader.GetAllMajors().ToList();

            if (majorSet.Count == 0)
            {
                return;
            }
            var id = majorSet.First().ID.Value;



            SchoolMajorReader.GetMajor(id);
        }


        [TestMethod()]
        public void GetMajorsBySchoolTest()
        {
            TestGlobal.TestInit();

            var schoolID = 1;   //CU Boulder


            var majorSet = SchoolMajorReader.GetMajorsBySchool(schoolID).ToList();
            majorSet.ForEach(x => Console.WriteLine(x.Name));
        }


        [TestMethod()]
        public void GetMajorsByEmailTest()
        {
            TestGlobal.TestInit();

            var email = "aual1780@colorado.edu";

            var majorSet = SchoolMajorReader.GetMajorsByEmail(email).ToList();
            majorSet.ForEach(x => Console.WriteLine(x.Name));
        }


        [TestMethod()]
        public void GetMajorsByDomainTest()
        {
            TestGlobal.TestInit();

            var email = "aual1780@colorado.edu";   //CU Boulder
            var domain = email.Substring(email.IndexOf("@"));   //@colorado.edu


            var majorSet = SchoolMajorReader.GetMajorsByDomain(domain).ToList();
            majorSet.ForEach(x => Console.WriteLine(x.Name));
        }
    }
}