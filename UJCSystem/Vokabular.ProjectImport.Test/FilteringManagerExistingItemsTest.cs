using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;
using Project = Vokabular.DataEntities.Database.Entities.Project;

namespace Vokabular.ProjectImport.Test
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

            var serviceProvider = new MockIocContainer().CreateServiceProvider();
            m_parser = serviceProvider.GetService<IProjectParser>();
            m_importedRecord = new ImportedRecord { RawData =  GetRecord("OaiPmh_Marc21_JanHus.xml") };
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

        private string GetRecord(string name)
        {
            var xml = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + name);
            var oaiPmhRecord = xml.XmlDeserializeFromString<OAIPMHType>();
            var record = ((GetRecordType)oaiPmhRecord.Items.First()).record;
            return record.metadata.OuterXml;
        }
    }
}
