using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Security.Accounts.APIControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Schools.Management;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Entities.Users.Management;

namespace UHub.CoreLib.Security.Accounts.Tests
{
    [TestClass()]
    public class AccountControllerTests
    {
        [TestMethod()]
        public void CreateUserTest()
        {
            TestGlobal.TestInit();

            string status;

            //INVALID DOMAIN
            var testUser1 = new User_C_PublicDTO()
            {
                Email = "test@test.test",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser1);
            Assert.AreEqual("Email Domain Not Supported", status);


            //EMPTY EMAIL
            var testUser2 = new User_C_PublicDTO()
            {
                Email = "",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser2);
            Assert.AreEqual("Email Empty", status);


            //DUPLICATE EMAIL
            var testUser3 = new User_C_PublicDTO()
            {
                Email = "aual1780@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser3);
            Assert.AreEqual("Email Duplicate", status);


            //EMPTY PASSWORD
            var testUser4 = new User_C_PublicDTO()
            {
                Email = "TEST123456@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = "",
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser4);
            Assert.AreEqual("Password Empty", status);


            //INVALID PASSWORD
            var testUser5 = new User_C_PublicDTO()
            {
                Email = "TEST123456@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = "1234",
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser5);
            Assert.AreEqual("Password Invalid", status);


            //INVALID MAJOR
            var testUser6 = new User_C_PublicDTO()
            {
                Email = "TEST123456@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Sciences",
            };
            status = CreateUserTestWorker(testUser6);
            Assert.AreEqual("Major Invalid", status);


            //VALID
            var email = Guid.NewGuid().ToString() + "@colorado.edu";
            var testUser7 = new User_C_PublicDTO()
            {
                Email = email,
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser7);
            Assert.AreEqual("User Created", status);


            UserWriter.TryPurgeUser(email);

        }

        private string CreateUserTestWorker(User_C_PublicDTO testUser)
        {
            var tmpUser = testUser.ToInternal<User>();


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            string status = "Account Creation Failed";


            try
            {
                var result = CoreFactory.Singleton.Accounts.TryCreateUser(
                    tmpUser, 
                    true,
                    GeneralFailHandler: (code) =>
                    {
                        if (enableFailCode)
                        {
                            status = code.ToString();
                        }
                    },
                    SuccessHandler: (newUser, canLogin) =>
                    {
                        status = "User Created";
                    });


                if (result != AccountResultCode.Success && enableDetail)
                {
                    switch (result)
                    {
                        case AccountResultCode.EmailEmpty: { status = "Email Empty"; break; }
                        case AccountResultCode.EmailInvalid: { status = "Email Invalid"; break; }
                        case AccountResultCode.EmailDuplicate: { status = "Email Duplicate"; break; }
                        case AccountResultCode.EmailDomainInvalid: { status = "Email Domain Not Supported"; break; }
                        case AccountResultCode.UsernameDuplicate: { status = "Username Duplicate"; break; }
                        case AccountResultCode.MajorInvalid: { status = "Major Invalid"; break; }
                        case AccountResultCode.PswdEmpty: { status = "Password Empty"; break; }
                        case AccountResultCode.PswdInvalid: { status = "Password Invalid"; break; }
                    }
                }


            }
            catch (Exception ex)
            {
                throw;
            }


            return status;
        }
    }
}