using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;
using Project = Vokabular.DataEntities.Database.Entities.Project;

namespace Vokabular.ProjectImport.Test.UnitTests
{
    [TestClass]
    public class FilteringManagerExistingItemsTest
    {
        private FilteringManager m_filteringManager;
        private IProjectParser m_parser;
        private ImportedRecord m_importedRecord;
        private Mock<ImportedProjectMetadataManager> m_importedProjectMetadataManagerMock;
        private Mock<ImportHistoryManager> m_importHistoryManagerMock;

        [TestInitialize]
        public void Init()
        {
            var mockFactory = new MockRepository(MockBehavior.Loose) { CallBase = true };
            m_importedProjectMetadataManagerMock = mockFactory.Create<ImportedProjectMetadataManager>(new object[1]);
            m_importHistoryManagerMock = mockFactory.Create<ImportHistoryManager>(new object[1]);
            
            m_importedProjectMetadataManagerMock.Setup(x => x.GetImportedProjectMetadataByExternalId(It.IsAny<string>())).
                Returns(new ImportedProjectMetadata{ExternalId = "1", Project = new Project{Id = 1}});

            m_filteringManager = new FilteringManager(m_importedProjectMetadataManagerMock.Object, m_importHistoryManagerMock.Object);

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
        public void HistoryDoesNotExistTest()
        {
            m_importHistoryManagerMock.Setup(x => x.GetLastImportHistoryForImportedProjectMetadata(It.IsAny<int>())).Returns(() => null);           

            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"%Hus%"}}};

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(false, m_importedRecord.IsNew);
            Assert.AreEqual(true, m_importedRecord.IsSuitable);
        }

        [TestMethod]
        public void NewRecordTest()
        {
            m_importHistoryManagerMock.Setup(x => x.GetLastImportHistoryForImportedProjectMetadata(It.IsAny<int>())).Returns(new ImportHistory{Date = DateTime.UtcNow});           
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"%Hus%"}}};
            m_importedRecord.TimeStamp = DateTime.UtcNow.AddHours(1);

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(false, m_importedRecord.IsNew);
            Assert.AreEqual(true, m_importedRecord.IsSuitable);
        }

        [TestMethod]
        public void OldRecordTest()
        {
            m_importHistoryManagerMock.Setup(x => x.GetLastImportHistoryForImportedProjectMetadata(It.IsAny<int>())).Returns(new ImportHistory{Date = DateTime.UtcNow});           
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"%Hus%"}}};
            m_importedRecord.TimeStamp = DateTime.UtcNow.AddHours(-1);

            m_filteringManager.SetFilterData(m_importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(false, m_importedRecord.IsNew);
            Assert.AreEqual(false, m_importedRecord.IsSuitable);
        }
    }
}
