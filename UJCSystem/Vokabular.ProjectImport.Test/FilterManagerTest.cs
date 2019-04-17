using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.ProjectImport.Test
{
    [TestClass]
    public class FilterManagerTest
    {
        private FilteringManager m_filteringManager;
        private IProjectParser m_parser;
        private string m_metadata;

        [TestInitialize]
        public void Init()
        {
            
            var mockFactory = new MockRepository(MockBehavior.Loose) { CallBase = true };
            var importedProjectMetadataRepositoryMock = mockFactory.Create<ImportedProjectMetadataRepository>(new object[1]);
            var importHistoryRepositoryMock = mockFactory.Create<ImportHistoryRepository>(new object[1]);
            //TODO mock only managers and insert them
            
            importedProjectMetadataRepositoryMock.Setup(x => x.GetImportedProjectMetadata(It.IsAny<string>())).
                Returns(new ImportedProjectMetadata(){ExternalId = "a"});

          //  importHistoryRepositoryMock.Setup(x => x.GetLastImportHistoryForImportedProjectMetadata(It.IsAny<int>())).Returns(null);

            var services = new ServiceCollection();

            

            var serviceProvider = MockIocFactory.CreateMockIocContainer();
            m_filteringManager = serviceProvider.GetService<FilteringManager>();
            m_parser = serviceProvider.GetService<IProjectParser>();
            m_metadata = GetRecord("OaiPmh_Marc21_JanHus.xml");
        }

        [TestMethod]
        public void TestMethod1()
        {
            var importedRecord = new ImportedRecord { RawData = m_metadata };
            var filteringExpressions = new Dictionary<string, List<string>> {{"100a", new List<string> {"Hus, Jan,"}}};

            m_filteringManager.SetFilterData(importedRecord, filteringExpressions, m_parser);

            Assert.AreEqual(true, importedRecord.IsSuitable);
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
