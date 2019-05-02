using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.ProjectImport.ImportPipeline
{
    public class ImportPipelineBuilder
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<string, IProjectParser> m_projectParsers;
        private readonly ImportManager m_importManager;
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;
        private readonly ProjectRepository m_projectRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly IServiceProvider m_serviceProvider;

        public ImportPipelineBuilder(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IProjectParser> parsers,
            ImportManager importManager, FilteringExpressionSetManager filteringExpressionSetManager,
            ProjectRepository projectRepository, PermissionRepository permissionRepository, IServiceProvider serviceProvider)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_projectParsers = new Dictionary<string, IProjectParser>();
            m_importManager = importManager;
            m_filteringExpressionSetManager = filteringExpressionSetManager;
            m_serviceProvider = serviceProvider;
            m_projectRepository = projectRepository;
            m_permissionRepository = permissionRepository;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ExternalRepositoryTypeName, manager);
            }

            foreach (var parser in parsers)
            {
                m_projectParsers.Add(parser.BibliographicFormatName, parser);
            }
        }

        public ImportPipeline Build(ExternalRepositoryDetailContract externalRepository, int importHistoryId,
            RepositoryImportProgressInfo progressInfo, CancellationToken cancellationToken)
        {
            var executionOptions = new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken};

            var bufferBlock = new BufferBlock<object>(executionOptions);
            var responseParserBlock = BuildResponseParserBlock(externalRepository.ExternalRepositoryType.Name, executionOptions);
            var filterBlock = BuildFilterBlock(externalRepository.Id, externalRepository.BibliographicFormat.Name, executionOptions);
            var projectParserBlock = BuildProjectParserBlock(externalRepository.BibliographicFormat.Name, executionOptions);
            var nullTargetBlock = BuildNullTargetBlock(progressInfo, executionOptions);
            var saveBlock = BuildSaveBlock(m_importManager.UserId, importHistoryId, externalRepository.Id, progressInfo, executionOptions);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};

            bufferBlock.LinkTo(responseParserBlock, linkOptions);
            responseParserBlock.LinkTo(filterBlock, linkOptions);
            filterBlock.LinkTo(projectParserBlock, linkOptions, importedRecord => importedRecord.IsSuitable);
            filterBlock.LinkTo(nullTargetBlock, linkOptions);
            projectParserBlock.LinkTo(saveBlock, linkOptions);

            return new ImportPipeline {BufferBlock = bufferBlock, LastBlock = saveBlock};
        }

        public TransformBlock<object, ImportedRecord> BuildResponseParserBlock(string repositoryType,
            ExecutionDataflowBlockOptions executionOptions)
        {
            m_projectImportManagers.TryGetValue(repositoryType, out var importManager);
            if (importManager == null)
            {
                throw new ImportFailedException($"Import manager was not found for repository type {repositoryType}.");
            }

            return new TransformBlock<object, ImportedRecord>(
                response => importManager.ParseResponse(response),
                executionOptions
            );
        }

        public TransformBlock<ImportedRecord, ImportedRecord> BuildFilterBlock(int externalRepositoryId, string formatName,
            ExecutionDataflowBlockOptions executionOptions)
        {
            m_projectParsers.TryGetValue(formatName, out var parser);
            if (parser == null)
            {
                throw new ImportFailedException($"Project parser was not found for bibliographic format {formatName}.");
            }

            var filteringExpressions =
                m_filteringExpressionSetManager.GetFilteringExpressionsByExternalRepository(externalRepositoryId);

            return new TransformBlock<ImportedRecord, ImportedRecord>(importedRecord =>
                {
                    if (importedRecord.IsFailed)
                    {
                        return importedRecord;
                    }

                    using (var scope = m_serviceProvider.CreateScope())
                    {
                        var filteringManager = scope.ServiceProvider.GetRequiredService<FilteringManager>();
                        return filteringManager.SetFilterData(importedRecord, filteringExpressions, parser);
                    }
                }, executionOptions
            );
        }


        public TransformBlock<ImportedRecord, ImportedRecord> BuildProjectParserBlock(string bibliographicFormat,
            ExecutionDataflowBlockOptions executionOptions)
        {
            m_projectParsers.TryGetValue(bibliographicFormat, out var parser);
            if (parser == null)
            {
                throw new ImportFailedException($"Project parser was not found for bibliographic format {bibliographicFormat}.");
            }

            return new TransformBlock<ImportedRecord, ImportedRecord>(importedRecord =>
                {
                    if (importedRecord.IsFailed)
                    {
                        return importedRecord;
                    }

                    return parser.AddParsedProject(importedRecord);
                },
                executionOptions
            );
        }

        public ActionBlock<ImportedRecord> BuildSaveBlock(int userId, int importHistoryId, int externalRepositoryId,
            RepositoryImportProgressInfo progressInfo, ExecutionDataflowBlockOptions executionOptions)
        {
            var bookTypeId = m_projectRepository.InvokeUnitOfWork(x => x.GetBookTypeByEnum(BookTypeEnum.BibliographicalItem)).Id;

            var specialPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissions());
            var importPermissions = specialPermissions.OfType<ReadExternalProjectPermission>().ToList();
            var groupsWithPermissionIds = new List<int>();

            if (importPermissions.Count != 0)
            {
                groupsWithPermissionIds = m_permissionRepository.InvokeUnitOfWork(x => x.GetGroupsBySpecialPermissionIds(importPermissions.Select(y => y.Id))).Select(x => x.Id).ToList();
            }
            

            return new ActionBlock<ImportedRecord>(importedRecord =>
                {
                    using (var scope = m_serviceProvider.CreateScope())
                    {
                        var importedProjectManager = scope.ServiceProvider.GetRequiredService<ImportedProjectManager>();
                        importedProjectManager.SaveImportedProject(importedRecord, userId, externalRepositoryId, bookTypeId,
                            groupsWithPermissionIds, importHistoryId, progressInfo);
                    }
                },
                executionOptions
            );
        }

        public ActionBlock<ImportedRecord> BuildNullTargetBlock(RepositoryImportProgressInfo progressInfo,
            ExecutionDataflowBlockOptions executionOptions)
        {
            return new ActionBlock<ImportedRecord>(
                importedRecord => { progressInfo.IncrementProcessedProjectsCount(); }, executionOptions
            );
        }
    }
}