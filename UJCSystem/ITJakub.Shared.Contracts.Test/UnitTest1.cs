using System.Collections.Generic;
using ITJakub.SearchService.DataContracts.Contracts.SearchResults;
using ITJakub.Shared.Contracts.Searching.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.Shared.DataContracts.Search.Old;

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

        [TestMethod]
        public void CorpusSearchResultContractListDeserializationTest()
        {
            var stringResult = "<CorpusSearchResultContractList xmlns=\"http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><SearchResults><CorpusSearchResultContract><BookXmlId>{E494DBC5-F3C4-4841-B4D3-C52FE99839EB}</BookXmlId><PageResultContext><ContextStructure><After>tvój.Protož konečně pravím to, že nikakež ode...</After><Before>...však s’ z břicha mého vyšel. Já jsem doma, ale ne </Before><Match>otec</Match></ContextStructure><PageName>1v</PageName><PageXmlId>t-1.body-1.div-1.div-1.p-1.pb-1</PageXmlId><VerseXmlId/><VerseName/></PageResultContext><VersionXmlId>0c6bfd7e-67d9-460d-89d0-90c069075969</VersionXmlId></CorpusSearchResultContract></SearchResults></CorpusSearchResultContractList>";
            var corpusList = CorpusSearchResultContractList.FromXml(stringResult);
            Assert.IsNotNull(corpusList);
        }


        [TestMethod]
        public void CorpusSearchResultContractDeserializationTest()
        {
            var notes = "<Notes xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>poznámka textová</a:string><a:string>cěstú</a:string></Notes>";
            var stringResult = "<CorpusSearchResultContract xmlns=\"http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results\"><BookXmlId>{E494DBC5-F3C4-4841-B4D3-C52FE99839EB}</BookXmlId>"+notes+"<PageResultContext><ContextStructure><After>tvój.Protož konečně pravím to, že nikakež ode...</After><Before>...však s’ z břicha mého vyšel. Já jsem doma, ale ne </Before><Match>otec</Match></ContextStructure><PageName>1v</PageName><PageXmlId>t-1.body-1.div-1.div-1.p-1.pb-1</PageXmlId><VerseXmlId/><VerseName/></PageResultContext><VersionXmlId>0c6bfd7e-67d9-460d-89d0-90c069075969</VersionXmlId></CorpusSearchResultContract>";
            var corpusResult = CorpusSearchResultContract.FromXml(stringResult);
            Assert.IsNotNull(corpusResult);
        }
    }
}
