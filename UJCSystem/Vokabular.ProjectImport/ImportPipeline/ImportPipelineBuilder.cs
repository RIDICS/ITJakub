using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.HttpClient.Client.Auth;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;
using Vokabular.Shared.Const;

namespace Vokabular.ProjectImport.ImportPipeline
{
    public class ImportPipelineBuilder
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<string, IProjectParser> m_projectParsers;
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;
        private readonly ProjectRepository m_projectRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly IServiceProvider m_serviceProvider;
        private readonly PermissionApiClient m_permissionApiClient;

        public ImportPipelineBuilder(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IProjectParser> parsers,
            FilteringExpressionSetManager filteringExpressionSetManager, ProjectRepository projectRepository,
            PermissionRepository permissionRepository, IServiceProvider serviceProvider, PermissionApiClient permissionApiClient)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_projectParsers = new Dictionary<string, IProjectParser>();
            m_filteringExpressionSetManager = filteringExpressionSetManager;
            m_serviceProvider = serviceProvider;
            m_permissionApiClient = permissionApiClient;
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

        public virtual TransformBlock<object, ImportedRecord> BuildResponseParserBlock(string repositoryType,
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

        public virtual TransformBlock<ImportedRecord, ImportedRecord> BuildFilterBlock(int externalRepositoryId, string formatName,
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


        public virtual TransformBlock<ImportedRecord, ImportedRecord> BuildProjectParserBlock(string bibliographicFormat,
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

        public virtual ActionBlock<ImportedRecord> BuildSaveBlock(int userId, int importHistoryId, int externalRepositoryId,
            RepositoryImportProgressInfo progressInfo, ExecutionDataflowBlockOptions executionOptions)
        {
            var bookTypeId = m_projectRepository.InvokeUnitOfWork(x => x.GetBookTypeByEnum(BookTypeEnum.BibliographicalItem)).Id;

            var allPermissions = m_permissionApiClient.GetAllPermissionsAsync().GetAwaiter().GetResult();
            var permission =
                allPermissions.SingleOrDefault(x => x.Name == $"{PermissionNames.AutoImport}{(int) BookTypeEnum.BibliographicalItem}");

            IList<int> roleIds = new List<int>();
            if (permission != null)
            {
                var roleExternalIds = permission.Roles.Select(x => x.Id);
                roleIds = m_permissionRepository.InvokeUnitOfWork(x => x.GetGroupIdsByExternalIds(roleExternalIds));
            }

            return new ActionBlock<ImportedRecord>(importedRecord =>
                {
                    using (var scope = m_serviceProvider.CreateScope())
                    {
                        var importedProjectManager = scope.ServiceProvider.GetRequiredService<ImportedProjectManager>();
                        importedProjectManager.SaveImportedProject(importedRecord, userId, externalRepositoryId, bookTypeId,
                            roleIds, importHistoryId, progressInfo);
                    }
                },
                executionOptions
            );
        }

        public virtual ActionBlock<ImportedRecord> BuildNullTargetBlock(RepositoryImportProgressInfo progressInfo,
            ExecutionDataflowBlockOptions executionOptions)
        {
            return new ActionBlock<ImportedRecord>(
                importedRecord => { progressInfo.IncrementProcessedProjectsCount(); }, executionOptions
            );
        }
    }
}