using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using ServicesLayer;
using AdvLib;

namespace UnitTests
{
    [TestClass]
    public class ExtractServiceTest
    {
        static private readonly string AVDFILE_TEST01 = @"/data/test01.avd";

        private string getFileFullPath(string filename)
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + filename;
        }

        [TestMethod]
        public void TestExtractAvd()
        {

            string file = getFileFullPath(AVDFILE_TEST01);
            Debug.Write(file);

            FileStream fileStream = File.OpenRead(file);
            AdvFilesService extractService = new AdvFilesService();
            AdvLib.AdvFile test01Avd = (AdvLib.AdvFile)extractService.Unpack(fileStream);

            Assert.IsTrue(test01Avd.HasFile("meta.xml"));
            Assert.IsTrue(test01Avd.GetFile("meta.xml").Length > 0);

            Assert.IsTrue(test01Avd.HasFile("original.xml"));
            Assert.IsTrue(test01Avd.GetFile("original.xml").Length > 0);
        }
    }
}
