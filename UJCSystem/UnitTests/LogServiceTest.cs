using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesLayer;

namespace UnitTests
{
    /// <summary>
    /// Summary description for LogServiceTest
    /// </summary>
    [TestClass]
    public class LogServiceTest
    {
        
        [TestMethod]
        public void TestLogger()
        {
            LogService logService = new LogService();
            logService.LogDebug("This is test debug log");
        }
    }
}
