using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib;
using UHub.CoreLib.SmtpInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.SmtpInterop.Tests
{
    [TestClass]
    public class SmtpInteropTests
    {

        [TestMethod]
        public void SendMessageTest()
        {
            TestGlobal.TestInit();


            var msg = new SmtpMessage_ConfirmAcct($"Confirm U-HUB Account (TEST)", "U-HUB", "aual1780@colorado.edu")
            {
                ConfirmationURL = "https://u-hub.life"
            };


            var val = SmtpManager.TrySendMessage(msg);
            Assert.IsTrue(val);

        }
    }
}
