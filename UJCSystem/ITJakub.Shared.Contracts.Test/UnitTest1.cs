using System;
using System.Collections.Generic;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ITJakub.Shared.Contracts.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var xml = new SearchResultContractList
            {
                SearchResults = new List<SearchResultContract>
                {
                    new SearchResultContract{ BookXmlId = "bookXmlId1"},new SearchResultContract{ BookXmlId = "bookXmlId2"}
                }
            }.ToXml();
        }
    }
}
