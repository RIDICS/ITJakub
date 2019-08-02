using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.ImportPipeline;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.ProjectImport.Test.UnitTests
{
    [TestClass]
    public class ImportPipelineDirectorTest
    {
        private ImportPipelineDirector m_importPipelineDirector;
        private Mock<ImportPipelineBuilder> m_importPipelineBuilderMock;
        private readonly List<string> m_buildCallOrder = new List<string>();
        private readonly List<string> m_processCallOrder = new List<string>();
        private ActionBlock<ImportedRecord> m_saveBlock;
        private const string ResponseParserBlockId = "1";
        private const string FilterBlockId = "2";
        private const string ProjectParserBlockId = "3";
        private const string NullBlockId = "4";
        private const string SaveBlockId = "5";


        [TestInitialize]
        public void Init()
        {
            var mockFactory = new MockRepository(MockBehavior.Loose) {CallBase = true};

            m_importPipelineBuilderMock =
                mockFactory.Create<ImportPipelineBuilder>(new List<IProjectImportManager>(), new List<IProjectParser>(), null, null, null,
                    null, null);

            m_importPipelineBuilderMock
                .Setup(x => x.BuildResponseParserBlock(It.IsAny<string>(), It.IsAny<ExecutionDataflowBlockOptions>()))
                .Returns(new TransformBlock<object, ImportedRecord>(record =>
                {
                    m_processCallOrder.Add(ResponseParserBlockId);
                    return (ImportedRecord) record;
                }))
                .Callback(() => m_buildCallOrder.Add(ResponseParserBlockId));

            m_importPipelineBuilderMock
                .Setup(x => x.BuildFilterBlock(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ExecutionDataflowBlockOptions>()))
                .Returns(new TransformBlock<ImportedRecord, ImportedRecord>(record =>
                {
                    m_processCallOrder.Add(FilterBlockId);
                    return record;
                }))
                .Callback(() => m_buildCallOrder.Add(FilterBlockId));

            m_importPipelineBuilderMock
                .Setup(x => x.BuildProjectParserBlock(It.IsAny<string>(), It.IsAny<ExecutionDataflowBlockOptions>()))
                .Returns(new TransformBlock<ImportedRecord, ImportedRecord>(record =>
                {
                    m_processCallOrder.Add(ProjectParserBlockId);
                    return record;
                }))
                .Callback(() => m_buildCallOrder.Add(ProjectParserBlockId));

            m_saveBlock = new ActionBlock<ImportedRecord>(record =>
            {
                m_processCallOrder.Add(SaveBlockId);
                m_saveBlock.Complete();
            });

            m_importPipelineBuilderMock
                .Setup(x => x.BuildNullTargetBlock(It.IsAny<RepositoryImportProgressInfo>(), It.IsAny<ExecutionDataflowBlockOptions>()))
                .Returns(new ActionBlock<ImportedRecord>(record =>
                {
                    m_processCallOrder.Add(NullBlockId);
                    m_saveBlock.Complete();
                }))
                .Callback(() => m_buildCallOrder.Add(NullBlockId));
           
            m_importPipelineBuilderMock
                .Setup(x => x.BuildSaveBlock(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RepositoryImportProgressInfo>(),
                    It.IsAny<ExecutionDataflowBlockOptions>()))
                .Returns(m_saveBlock)
                .Callback(() => m_buildCallOrder.Add(SaveBlockId));

            var importManagerMock = mockFactory.Create<ImportManager>();
            importManagerMock.Setup(x => x.UserId).Returns(1);

            m_importPipelineDirector = new ImportPipelineDirector(importManagerMock.Object);
        }

        [TestMethod]
        public void BuildImportPipelineTest()
        {
            var cancellationToken = CancellationToken.None;
            var importPipeline = m_importPipelineDirector.Build(m_importPipelineBuilderMock.Object, new ExternalRepositoryDetailContract
                {
                    BibliographicFormat = new BibliographicFormatContract(),
                    ExternalRepositoryType = new ExternalRepositoryTypeContract()
                },
                1, new RepositoryImportProgressInfo(1, "Test"), cancellationToken);
            importPipeline.BufferBlock.Post(new object());

            CollectionAssert.AreEqual(new List<string>{ResponseParserBlockId, FilterBlockId, ProjectParserBlockId, NullBlockId, SaveBlockId}, m_buildCallOrder);
            Assert.AreSame(m_saveBlock, importPipeline.LastBlock);
        }

        [TestMethod]
        public void PostFailedRecordToImportPipelineTest()
        {
            var cancellationToken = CancellationToken.None;
            var importPipeline = m_importPipelineDirector.Build(m_importPipelineBuilderMock.Object, new ExternalRepositoryDetailContract
                {
                    BibliographicFormat = new BibliographicFormatContract(),
                    ExternalRepositoryType = new ExternalRepositoryTypeContract()
                },
                1, new RepositoryImportProgressInfo(1, "Test"), cancellationToken);
            importPipeline.BufferBlock.Post(new ImportedRecord {IsSuitable = false});
            importPipeline.LastBlock.Completion.Wait();

            CollectionAssert.AreEqual(new List<string>{ResponseParserBlockId, FilterBlockId, NullBlockId}, m_processCallOrder);
        }

        [TestMethod]
        public void PostSuccessRecordToImportPipelineTest()
        {
            var cancellationToken = CancellationToken.None;
            var importPipeline = m_importPipelineDirector.Build(m_importPipelineBuilderMock.Object, new ExternalRepositoryDetailContract
                {
                    BibliographicFormat = new BibliographicFormatContract(),
                    ExternalRepositoryType = new ExternalRepositoryTypeContract()
                },
                1, new RepositoryImportProgressInfo(1, "Test"), cancellationToken);
            importPipeline.BufferBlock.Post(new ImportedRecord {IsSuitable = true});
            importPipeline.LastBlock.Completion.Wait();

            CollectionAssert.AreEqual(new List<string>{ResponseParserBlockId, FilterBlockId, ProjectParserBlockId, SaveBlockId}, m_processCallOrder);
        }
    }
}