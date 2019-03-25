using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;
using Vokabular.Shared.Extensions;

namespace Vokabular.ProjectImport.ImportPipeline
{
    public class ImportPipelineBuilder
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<string, IProjectParser> m_projectParsers;
        private readonly ImportManager m_importManager;
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;
        private readonly ImportMetadataManager m_importMetadataManager;
        private readonly ProjectManager m_projectManager;
        private readonly ILogger<ImportPipelineBuilder> m_logger;
        private readonly FilteringManager m_filteringManager;

        public ImportPipelineBuilder(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IProjectParser> parsers,
            ImportManager importManager, FilteringExpressionSetManager filteringExpressionSetManager,
            ImportMetadataManager importMetadataManager, ProjectManager projectManager, ILogger<ImportPipelineBuilder> logger,
            FilteringManager filteringManager)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_projectParsers = new Dictionary<string, IProjectParser>();
            m_importManager = importManager;
            m_filteringExpressionSetManager = filteringExpressionSetManager;
            m_importMetadataManager = importMetadataManager;
            m_projectManager = projectManager;
            m_logger = logger;
            m_filteringManager = filteringManager;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ExternalRepositoryTypeName, manager);
            }

            foreach (var parser in parsers)
            {
                m_projectParsers.Add(parser.BibliographicFormatName, parser);
            }
        }

        public ImportPipeline Build(ExternalRepository externalRepository, ImportHistory importHistory,
            RepositoryImportProgressInfo progressInfo, CancellationToken cancellationToken)
        {
            var executionOptions = new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken};

            var bufferBlock = new BufferBlock<object>(executionOptions);
            var responseParserBlock = BuildResponseParserBlock(externalRepository.ExternalRepositoryType.Name, executionOptions);
            var filterBlock = BuildFilterBlock(externalRepository.Id, externalRepository.BibliographicFormat.Name, executionOptions);
            var projectParserBlock = BuildProjectParserBlock(externalRepository.BibliographicFormat.Name, executionOptions);
            var nullTargetBlock = BuildNullTargetBlock(progressInfo, executionOptions);
            var saveBlock = BuildSaveBlock(m_importManager.UserId, importHistory, progressInfo, executionOptions);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};

            bufferBlock.LinkTo(responseParserBlock, linkOptions);
            responseParserBlock.LinkTo(filterBlock, linkOptions);
            filterBlock.LinkTo(projectParserBlock, linkOptions, projectMetadata => projectMetadata.IsSuitable);
            filterBlock.LinkTo(nullTargetBlock, linkOptions);
            projectParserBlock.LinkTo(saveBlock, linkOptions);

            return new ImportPipeline {BufferBlock = bufferBlock, LastBlock = saveBlock};
        }

        public TransformBlock<ProjectImportMetadata, ProjectImportMetadata> BuildFilterBlock(int externalRepositoryId, string formatName,
            ExecutionDataflowBlockOptions executionOptions)
        {
            m_projectParsers.TryGetValue(formatName, out var parser);
            if (parser == null)
            {
                throw new ImportFailedException($"Project parser was not found for bibliographic format {formatName}.");
            }

            var filteringExpressions =
                m_filteringExpressionSetManager.GetFilteringExpressionsByExternalRepository(externalRepositoryId);

            return new TransformBlock<ProjectImportMetadata, ProjectImportMetadata>(metadata =>
                {
                    if (metadata.IsFailed)
                    {
                        return metadata;
                    }

                    return m_filteringManager.Filter(metadata, filteringExpressions, parser);
                }, executionOptions
            );
        }

        public TransformBlock<object, ProjectImportMetadata> BuildResponseParserBlock(string repositoryType,
            ExecutionDataflowBlockOptions executionOptions)
        {
            m_projectImportManagers.TryGetValue(repositoryType, out var importManager);
            if (importManager == null)
            {
                throw new ImportFailedException($"Import manager was not found for repository type {repositoryType}.");
            }

            return new TransformBlock<object, ProjectImportMetadata>(
                response => importManager.ParseResponse(response),
                executionOptions
            );
        }

        public TransformBlock<ProjectImportMetadata, ProjectImportMetadata> BuildProjectParserBlock(string bibliographicFormat,
            ExecutionDataflowBlockOptions executionOptions)
        {
            m_projectParsers.TryGetValue(bibliographicFormat, out var parser);
            if (parser == null)
            {
                throw new ImportFailedException($"Project parser was not found for bibliographic format {bibliographicFormat}.");
            }

            return new TransformBlock<ProjectImportMetadata, ProjectImportMetadata>(
                projectImportMetadata => parser.Parse(projectImportMetadata),
                executionOptions
            );
        }

        public ActionBlock<ProjectImportMetadata> BuildSaveBlock(int userId, ImportHistory importHistory,
            RepositoryImportProgressInfo progressInfo, ExecutionDataflowBlockOptions executionOptions)
        {
            return new ActionBlock<ProjectImportMetadata>(projectImportMetadata =>
                {
                    try
                    {
                        if (!projectImportMetadata.IsFailed)
                        {
                            m_projectManager.SaveImportedProject(projectImportMetadata, userId);
                        }
                    }
                    catch (DataException e)
                    {
                        projectImportMetadata.IsFailed = true;
                        projectImportMetadata.FaultedMessage = e.Message;

                        if (m_logger.IsErrorEnabled())
                            m_logger.LogError(e, e.Message);
                    }
                    finally
                    {
                        m_importMetadataManager.CreateImportMetadata(projectImportMetadata, importHistory);
                        progressInfo.IncrementProcessedProjectsCount();
                    }
                },
                executionOptions
            );
        }

        public ActionBlock<ProjectImportMetadata> BuildNullTargetBlock(RepositoryImportProgressInfo progressInfo,
            ExecutionDataflowBlockOptions executionOptions)
        {
            return new ActionBlock<ProjectImportMetadata>(
                projectImportMetadata => { progressInfo.IncrementProcessedProjectsCount(); }, executionOptions
            );
        }
    }
}