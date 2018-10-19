using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub;
using System.Web.Mvc;

namespace UHub.Controllers.Tests
{
    [TestClass()] 
    public class AssemblyInfo
    {
        [TestMethod()]
        public void IndexTest()
        {
            var controller = new HomeController();
            var val = controller.Index() as ViewResult;


            Assert.IsNotNull(val);
        }
    }
}