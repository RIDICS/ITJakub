﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.ProjectImport.Permissions;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataEntities.UnitOfWork;

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
        private readonly IPermissionsProvider m_permissionsProvider;

        public ImportPipelineBuilder(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IProjectParser> parsers,
            FilteringExpressionSetManager filteringExpressionSetManager, ProjectRepository projectRepository,
            PermissionRepository permissionRepository, IServiceProvider serviceProvider, IPermissionsProvider permissionsProvider)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_projectParsers = new Dictionary<string, IProjectParser>();
            m_filteringExpressionSetManager = filteringExpressionSetManager;
            m_serviceProvider = serviceProvider;
            m_permissionsProvider = permissionsProvider;
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
                throw new ImportFailedException(MainServiceErrorCode.RepositoryImportManagerNotFound, $"The import manager was not found for repository type {repositoryType}.", repositoryType);
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
                throw new ImportFailedException(MainServiceErrorCode.ProjectParserNotFound, $"The project parser was not found for bibliographic format {formatName}.", formatName);
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
                throw new ImportFailedException(MainServiceErrorCode.ProjectParserNotFound, $"The project parser was not found for bibliographic format {bibliographicFormat}.", bibliographicFormat);
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

            var roleIds = m_permissionsProvider.GetRoleIdsByPermissionName($"{VokabularPermissionNames.AutoImport}{(int)BookTypeEnum.BibliographicalItem}") ??
                          new List<int>();

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