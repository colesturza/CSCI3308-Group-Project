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
        public void GetAllClubsTest()
        {
            TestGlobal.TestInit();

            var clubSet = SchoolClubReader.GetAllClubs().ToList();
            clubSet.ForEach(x => Console.WriteLine(x));
        }


        [TestMethod()]
        public void GetClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.GetAllClubs().ToList();

            if(clubSet.Count == 0)
            {
                return;
            }

            var id = clubSet.First().ID.Value;


            SchoolClubReader.GetClub(id);
        }



        [TestMethod()]
        public void GetClubsBySchoolTest()
        {
            TestGlobal.TestInit();

            var schoolID = 1;   //CU Boulder


            var clubSet = SchoolClubReader.GetClubsBySchool(schoolID).ToList();
            clubSet.ForEach(x => Console.WriteLine(x.Name));
        }



        [TestMethod()]
        public void GetClubsByEmailTest()
        {
            TestGlobal.TestInit();

            var email = "aual1780@colorado.edu";


            var clubSet= SchoolClubReader.GetClubsByEmail(email).ToList();
            clubSet.ForEach(x => Console.WriteLine(x.Name));
        }


        [TestMethod()]
        public void GetClubsByDomainTest()
        {
            TestGlobal.TestInit();

            var email= "aual1780@colorado.edu";   //CU Boulder
            var domain = email.Substring(email.IndexOf("@"));   //@colorado.edu


            var clubSet = SchoolClubReader.GetClubsByDomain(domain).ToList();
            clubSet.ForEach(x => Console.WriteLine(x.Name));
        }
    }
}