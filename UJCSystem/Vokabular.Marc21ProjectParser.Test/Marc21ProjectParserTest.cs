using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.Marc21ProjectParser.Test
{
    [TestClass]
    public class Marc21ProjectParserTest
    {
        private Marc21ProjectParser m_parser;
        private string m_metadata;

        [TestInitialize]
        public void Init()
        {
            var serviceProvider = MockIocFactory.CreateMockIocContainer();
            var parsers = serviceProvider.GetServices<IProjectParser>();
            m_parser = (Marc21ProjectParser) parsers.First(x => x.BibliographicFormatName == "Marc21");
            m_metadata = GetRecord("OaiPmh_Marc21_JanHus.xml");
        }

        [TestMethod]
        public void ParseProjectIdTest()
        {
            var importedRecord = new ImportedRecord { RawData = m_metadata };
            m_parser.AddParsedProject(importedRecord);

            Assert.AreEqual("kpm01361543", importedRecord.Project.Id);
        }

        [TestMethod]
        public void ParseAuthorTest()
        {
            var importedRecord = new ImportedRecord {RawData = m_metadata};
            m_parser.AddParsedProject(importedRecord);

            Assert.AreEqual(1, importedRecord.Project.Authors.Count);
            Assert.AreEqual("Jan", importedRecord.Project.Authors.First().FirstName);
            Assert.AreEqual("Hus", importedRecord.Project.Authors.First().LastName);
        }

        [TestMethod]
        public void ParseLiteraryGenreTest()
        {
            var importedRecord = new ImportedRecord { RawData = m_metadata };
            m_parser.AddParsedProject(importedRecord);

            Assert.AreEqual(3, importedRecord.Project.LiteraryGenres.Count);
            Assert.AreEqual("pojednání", importedRecord.Project.LiteraryGenres[0]);
            Assert.AreEqual("edice", importedRecord.Project.LiteraryGenres[1]);
            Assert.AreEqual("studie", importedRecord.Project.LiteraryGenres[2]);
        }

        [TestMethod]
        public void ParseProjectNameTest()
        {
            var importedRecord = new ImportedRecord { RawData = m_metadata };
            m_parser.AddParsedProject(importedRecord);

            Assert.AreEqual("Knihy kacířů se mají číst", importedRecord.Project.ProjectMetadata.Title);
        }

        [TestMethod]
        public void ParsePublishInfoTest()
        {
            var importedRecord = new ImportedRecord { RawData = m_metadata };
            m_parser.AddParsedProject(importedRecord);

            Assert.AreEqual("2015", importedRecord.Project.ProjectMetadata.PublishDate);
            Assert.AreEqual("Praha", importedRecord.Project.ProjectMetadata.PublishPlace);
            Assert.AreEqual("Kalich", importedRecord.Project.ProjectMetadata.PublisherText);
        }

        [TestMethod]
        public void ParseIncorrectInputTest()
        {
            Assert.ThrowsException<InvalidOperationException>(() => m_parser.AddParsedProject(new ImportedRecord { RawData = "IncorrectInput" }));
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
