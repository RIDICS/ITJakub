using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.IO;
using Ujc.Naki.DataLayer;
using ServicesLayer;

namespace UnitTests
{
    /// <summary>
    /// Summary description for LogServiceTest
    /// </summary>
    [TestClass]
    public class ValidationServiceTest
    {

        private IXmlDbDao dao = new ExistDao();

        [TestMethod]
        public void TestValidationSuccess()
        {

            string xml = Encoding.UTF8.GetString(dao.Read("000001-typo.xml"));
            ValidationService svc = new ValidationService();
            bool validated = svc.ValidateXml(xml);
            Assert.IsTrue(validated);
        }

        [TestMethod]
        public void TestValidationFail()
        {

            //string xml = Encoding.UTF8.GetString(dao.Read("000001-corrupt.xml"));
            //ValidationService svc = new ValidationService();
            //bool validated = svc.ValidateXml(xml);
            //Assert.IsFalse(validated);
        }

    }
}
