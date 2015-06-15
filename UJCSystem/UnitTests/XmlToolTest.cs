using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ujc.Naki.DataLayer;

namespace UnitTests
{
    /// <summary>
    /// Summary description for LogServiceTest
    /// </summary>
    [TestClass]
    public class XmlToolTest
    {

        [TestMethod]
        public void TestCutElements()
        {
            string xml = "<?xml version=\"1.0\"?> <exist:result xmlns:exist=\"foo\">" +
                "<my-result>XXX</my-result><my-result>YYY</my-result>" +
                "</exist:result>";
            string elemName = "my-result";
            string[] res = XmlTool.CutElementsText(xml, elemName);
            Assert.IsNotNull(res);
            Assert.AreEqual("XXX", res[0]);
            Assert.AreEqual("YYY", res[1]);
        }
    }
}
