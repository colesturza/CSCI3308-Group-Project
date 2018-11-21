using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Schools.DataInterop.Tests
{
    [TestClass]
    public class SchoolReaderTests
    {
        [TestMethod]
        public void GetAllSchoolsTest()
        {

            TestGlobal.TestInit();

            SchoolReader.GetAllSchools();

        }


        [TestMethod]
        public void GetSchoolTest()
        {

            TestGlobal.TestInit();

            var schoolSet = SchoolReader.GetAllSchools().ToList();


            if(schoolSet.Count == 0)
            {
                return;
            }

            var id = schoolSet.First().ID.Value;


            SchoolReader.GetSchool(id);
        }


        [TestMethod]
        public void GetSchoolByNameTest()
        {

            TestGlobal.TestInit();


            School school;
            school = SchoolReader.GetSchoolByName("CU Boulder");
            Assert.IsNotNull(school);


            school = SchoolReader.GetSchoolByName("hu98fp2qghp9jfkiwe");
            Assert.IsNull(school);


        }


        [TestMethod]
        public void GetSchoolByEmailTest()
        {

            TestGlobal.TestInit();


            School school;
            school = SchoolReader.GetSchoolByEmail("aual1780@colorado.edu");
            Assert.IsNotNull(school);


            school = SchoolReader.GetSchoolByEmail("test@colorado.edu");
            Assert.IsNotNull(school);


            school = SchoolReader.GetSchoolByEmail("test@test.test");
            Assert.IsNull(school);

        }


        [TestMethod]
        public void GetSchoolByDomainTest()
        {

            TestGlobal.TestInit();


            School school;
            school = SchoolReader.GetSchoolByEmail("@colorado.edu");
            Assert.IsNotNull(school);


            school = SchoolReader.GetSchoolByEmail("@test.test");
            Assert.IsNull(school);

        }


        [TestMethod]
        public void IsEmailValidTest()
        {

            TestGlobal.TestInit();


            bool isValid;
            isValid = SchoolReader.IsEmailValid("");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsEmailValid("asdafq");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsEmailValid("a");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsEmailValid("@colorado.edu");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsEmailValid("test@test.test");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsEmailValid("aual1780@colorado.edu");
            Assert.IsTrue(isValid);

        }


        [TestMethod]
        public void IsDomainValidTest()
        {

            TestGlobal.TestInit();


            bool isValid;
            isValid = SchoolReader.IsDomainValid("");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsDomainValid("asdafq");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsDomainValid("a");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsDomainValid("test@test.test");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsDomainValid("@test.test");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsDomainValid("aual1780@colorado.edu");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsDomainValid("colorado.edu");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.IsDomainValid("@colorado.edu");
            Assert.IsTrue(isValid);

        }
    }
}
