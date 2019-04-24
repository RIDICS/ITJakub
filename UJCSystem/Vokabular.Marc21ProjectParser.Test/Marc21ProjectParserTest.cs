using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;
using Vokabular.Shared.Const;

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

            Assert.AreEqual("kpm01361543", m_importedRecord.Project.Id);
        }

        [TestMethod]
        public void ParseAuthorTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual(1, m_importedRecord.Project.Authors.Count);
            Assert.AreEqual("Jan", m_importedRecord.Project.Authors.First().FirstName);
            Assert.AreEqual("Hus", m_importedRecord.Project.Authors.First().LastName);
        }

        [TestMethod]
        public void ParseLiteraryGenreTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual(3, m_importedRecord.Project.LiteraryGenres.Count);
            Assert.AreEqual("pojednání", m_importedRecord.Project.LiteraryGenres[0]);
            Assert.AreEqual("edice", m_importedRecord.Project.LiteraryGenres[1]);
            Assert.AreEqual("studie", m_importedRecord.Project.LiteraryGenres[2]);
        }

        [TestMethod]
        public void ParseProjectNameTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual("Knihy kacířů se mají číst", m_importedRecord.Project.ProjectMetadata.Title);
        }

        [TestMethod]
        public void ParsePublishInfoTest()
        {
            m_parser.AddParsedProject(m_importedRecord);

            Assert.AreEqual("2015", m_importedRecord.Project.ProjectMetadata.PublishDate);
            Assert.AreEqual("Praha", m_importedRecord.Project.ProjectMetadata.PublishPlace);
            Assert.AreEqual("Kalich", m_importedRecord.Project.ProjectMetadata.PublisherText);
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
