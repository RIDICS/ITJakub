using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ITJakub.Core.Database.Exist;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using IFilenamesResolver = Ujc.Naki.DataLayer.IFilenamesResolver;

namespace UnitTests
{
    /// <summary>
    /// Summary description for XQueryBuilderTest
    /// </summary>
    [TestClass]
    public class DefaultFilenamesResolverTest
    {

        private ExistDao dao;
        private DefaultFilenamesResolver m_filenames;

        [TestInitialize]
        public void init()
        {
            dao = new ExistDao();
            m_filenames = new DefaultFilenamesResolver(dao);
        }

        [TestMethod]
        public void TestGetIDFromName()
        {
            string name = "1234-typo.xml";
            int id = m_filenames.GetIDFromName(name);
            Assert.AreEqual(1234, id); // 1 result
        }

        [TestMethod]
        public void TestGetViewFromName()
        {
            string name = "1234-typo.xml";
            string view = m_filenames.GetViewFromName(name);
            Assert.AreEqual("typo", view); // 1 result
        }

        [TestMethod]
        public void TestGetView2FromName()
        {
            string name = "1234-img.png";
            string view = m_filenames.GetViewFromName(name);
            Assert.AreEqual("img", view); // 1 result
        }

        [TestMethod]
        public void TestGetView3FromName()
        {
            string name = "1234-imgpage-32v.png";
            string view = m_filenames.GetViewFromName(name);
            Assert.AreEqual("imgpage", view); // 1 result
        }

        [TestMethod]
        public void TestGetAllNames()
        {
            string[] names = m_filenames.GetAllNames();
            Assert.IsTrue(names.Length > 0);
        }

    }
}
