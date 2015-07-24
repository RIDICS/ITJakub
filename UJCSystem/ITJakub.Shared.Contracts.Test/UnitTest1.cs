using System.Collections.Generic;
using ITJakub.Shared.Contracts.Searching.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ITJakub.Shared.Contracts.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SearchResultContractSerializationTest()
        {
            var xml = new SearchResultContractList
            {
                SearchResults = new List<SearchResultContract>
                {
                    new SearchResultContract{ BookXmlId = "bookXmlId1"},new SearchResultContract{ BookXmlId = "bookXmlId2"}
                }
            }.ToXml();
            Assert.IsNotNull(xml);
        }
        
        [TestMethod]
        public void PageListContractSerializationTest()
        {
            var xml = new PageListContract
            {
                 PageList= new List<PageDescriptionContract>
                {
                    new PageDescriptionContract{ PageName = "name", PageXmlId = "xmlId1"},
                    new PageDescriptionContract{ PageName = "name2", PageXmlId = "xmlId2"},
                    new PageDescriptionContract{ PageName = "name3", PageXmlId = "xmlId3"},
                }
            }.ToXml();
            Assert.IsNotNull(xml);
        }
    }
}
