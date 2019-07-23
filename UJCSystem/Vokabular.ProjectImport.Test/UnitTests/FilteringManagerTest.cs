using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.ProjectImport.Test.UnitTests
{
    [TestClass]
    public class FilteringManagerTest
    {
        private FilteringManager m_filteringManager;
        private IProjectParser m_parser;
        private ImportedRecord m_importedRecord;

        [TestInitialize]
        public void Init()
        {
            var mockFactory = new MockRepository(MockBehavior.Loose) {CallBase = true};
            var importedProjectMetadataManagerMock = mockFactory.Create<ImportedProjectMetadataManager>(new object[1]);
            var importHistoryManagerMock = mockFactory.Create<ImportHistoryManager>(new object[1]);

            importedProjectMetadataManagerMock.Setup(x => x.GetImportedProjectMetadataByExternalId(It.IsAny<string>())).Returns(() => null);

            importHistoryManagerMock.Setup(x => x.GetLastImportHistoryForImportedProjectMetadata(It.IsAny<int>())).Returns(() => null);

            m_filteringManager = new FilteringManager(importedProjectMetadataManagerMock.Object, importHistoryManagerMock.Object);

            var parserMock = new Mock<IProjectParser>();
            parserMock.Setup(x => x.GetPairKeyValueList(It.IsAny<ImportedRecord>())).Returns(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("100a", "Èapek, Karel"),
                new KeyValuePair<string, string>("100a", "Hus, Jan,")
            });
            m_parser = parserMock.Object;
            m_importedRecord = new ImportedRecord();
        }

        [TestMethod]
        public void FilterEqualTest()
        {
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"Hus, Jan,"}}};

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(true, m_importedRecord.IsSuitable);
        }

        [TestMethod]
        public void FilterStartsWithTest()
        {
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"Hus%"}}};

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(true, m_importedRecord.IsSuitable);
        }

        [TestMethod]
        public void FilterEndsWithTest()
        {
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"%Jan,"}}};

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(true, m_importedRecord.IsSuitable);
        }

        [TestMethod]
        public void FilterContainsTest()
        {
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"%Hus%"}}};

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(true, m_importedRecord.IsSuitable);
        }

        [TestMethod]
        public void FilterNotContainsTest()
        {
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"%Petr%"}}};

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(false, m_importedRecord.IsSuitable);
        }

        [TestMethod]
        public void DeletedRecordTest()
        {
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"Hus, Jan,"}}};
            m_importedRecord.IsDeleted = true;
            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(false, m_importedRecord.IsSuitable);
        }
    }
}