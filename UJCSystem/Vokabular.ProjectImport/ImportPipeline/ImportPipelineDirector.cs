using System.Threading;
using System.Threading.Tasks.Dataflow;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;

namespace Vokabular.ProjectImport.ImportPipeline
{
    public class ImportPipelineDirector
    {
        private readonly ImportManager m_importManager;

        public ImportPipelineDirector(ImportManager importManager)
        {
            m_importManager = importManager;
        }

        public ImportPipeline Build(ImportPipelineBuilder importPipelineBuilder, ExternalRepositoryDetailContract externalRepository,
            int importHistoryId, RepositoryImportProgressInfo progressInfo, CancellationToken cancellationToken)
        {
            var executionOptions = new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken};

            var bufferBlock = new BufferBlock<object>(executionOptions);
            var responseParserBlock =
                importPipelineBuilder.BuildResponseParserBlock(externalRepository.ExternalRepositoryType.Name, executionOptions);
            var filterBlock = importPipelineBuilder.BuildFilterBlock(externalRepository.Id, externalRepository.BibliographicFormat.Name,
                executionOptions);
            var projectParserBlock =
                importPipelineBuilder.BuildProjectParserBlock(externalRepository.BibliographicFormat.Name, executionOptions);
            var nullTargetBlock = importPipelineBuilder.BuildNullTargetBlock(progressInfo, executionOptions);
            var saveBlock = importPipelineBuilder.BuildSaveBlock(m_importManager.UserId, importHistoryId, externalRepository.Id,
                progressInfo, executionOptions);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};

            bufferBlock.LinkTo(responseParserBlock, linkOptions);
            responseParserBlock.LinkTo(filterBlock, linkOptions);
            filterBlock.LinkTo(projectParserBlock, linkOptions, importedRecord => importedRecord.IsSuitable);
            filterBlock.LinkTo(nullTargetBlock, linkOptions);
            projectParserBlock.LinkTo(saveBlock, linkOptions);

            return new ImportPipeline {BufferBlock = bufferBlock, LastBlock = saveBlock};
        }
    }
}