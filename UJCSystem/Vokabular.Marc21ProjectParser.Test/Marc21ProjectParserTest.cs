using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Shared.Const;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.Marc21ProjectParser.Test
{
    [TestClass]
    public class Marc21ProjectParserTest
    {
        private Marc21ProjectParser m_parser;
        private ImportedRecord m_importedRecord;

        [TestInitialize]
        public void Init()
        {
            var serviceProvider = MockIocFactory.CreateMockIocContainer();
            var parsers = serviceProvider.GetServices<IProjectParser>();
            m_parser = (Marc21ProjectParser) parsers.First(x => x.BibliographicFormatName == BibliographicFormatNameConstant.Marc21);
            m_importedRecord = new ImportedRecord { RawData = GetRecord("OaiPmh_Marc21_JanHus.xml") };
        }

        [TestMethod]
        public void ParseProjectIdTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual("kpm01361543", m_importedRecord.ImportedProject.Id);
        }

        [TestMethod]
        public void ParseAuthorTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual(1, m_importedRecord.ImportedProject.Authors.Count);
            Assert.AreEqual("Jan", m_importedRecord.ImportedProject.Authors.First().FirstName);
            Assert.AreEqual("Hus", m_importedRecord.ImportedProject.Authors.First().LastName);
        }

        [TestMethod]
        public void ParseLiteraryGenreTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual(3, m_importedRecord.ImportedProject.LiteraryGenres.Count);
            Assert.AreEqual("pojednání", m_importedRecord.ImportedProject.LiteraryGenres[0]);
            Assert.AreEqual("edice", m_importedRecord.ImportedProject.LiteraryGenres[1]);
            Assert.AreEqual("studie", m_importedRecord.ImportedProject.LiteraryGenres[2]);
        }

        [TestMethod]
        public void ParseProjectNameTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual("Knihy kacířů se mají číst", m_importedRecord.ImportedProject.ProjectMetadata.Title);
        }

        [TestMethod]
        public void ParsePublishInfoTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual("2015", m_importedRecord.ImportedProject.ProjectMetadata.PublishDate);
            Assert.AreEqual("Praha", m_importedRecord.ImportedProject.ProjectMetadata.PublishPlace);
            Assert.AreEqual("Kalich", m_importedRecord.ImportedProject.ProjectMetadata.PublisherText);
        }

        [TestMethod]
        public void ParseIncorrectInputTest()
        {
            Assert.ThrowsException<InvalidOperationException>(() => m_parser.AddParsedProject(new ImportedRecord { RawData = "IncorrectInput" }));
        }

        private string GetRecord(string name)
        {
            var xml = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), name));
            var oaiPmhRecord = xml.XmlDeserializeFromString<OAIPMHType>();
            var record = ((GetRecordType)oaiPmhRecord.Items.First()).record;
            return record.metadata.OuterXml;
        }
    }
}
