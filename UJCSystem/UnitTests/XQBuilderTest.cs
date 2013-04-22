using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using Ujc.Naki.DataLayer;

using System.Net.Sockets;
using System.Diagnostics;

namespace UnitTests
{
    /// <summary>
    /// Summary description for XQueryBuilderTest
    /// </summary>
    [TestClass]
    public class XQBuilderTest
    {
        private IXmlDbDao dao;
        private IXmlFormatDescriptor formatDesc;
        private IFilenamesResolver filenames;

        [TestInitialize]
        public void init()
        {
            dao = new ExistDao();
            formatDesc = new TeiP5Descriptor();
            filenames = new DefaultFilenamesResolver(dao);
        }

        [TestMethod]
        public void TestSearchAlbertanusDocs()
        {
            XQBuilder queryBuilder = new XQBuilder(formatDesc, filenames);
            queryBuilder.SetCollectionName(dao.GetViewsCollection());
            queryBuilder.AddAuthorWhere("Albertanus");
            string qry = queryBuilder.Build();

            string result = dao.QueryXml(qry);
            Console.WriteLine("Found " + result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestSearch14CenturyDocs()
        {
            XQBuilder queryBuilder = new XQBuilder(formatDesc, filenames);
            queryBuilder.SetCollectionName(dao.GetViewsCollection());
            queryBuilder.AddDatationFromWhere(1301);
            queryBuilder.AddDatationToWhere(1400);
            string qry = queryBuilder.Build();

            string result = dao.QueryXml(qry);
            Console.WriteLine("Found " + result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetMetadata()
        {
            XQBuilder queryBuilder = new XQBuilder(formatDesc, filenames);
            queryBuilder.SetCollectionName(dao.GetViewsCollection());
            queryBuilder.SetForMetadataRetrieval(1); // ID == 1
            string qry = queryBuilder.Build();

            string result = dao.QueryXml(qry);
            Debug.WriteLine("Result: " + result);
            Assert.IsNotNull(result);
        }
    }
}
