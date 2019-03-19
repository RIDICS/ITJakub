using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectParsing.Parsers;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport
{
    public class ProjectImportHostedService : BackgroundService
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<string, IProjectParser> m_projectParsers;
        private readonly ImportManager m_importManager;
        private readonly ILogger<ProjectImportHostedService> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ProjectImportHostedService(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IProjectParser> parsers,
            ImportManager importManager, ILogger<ProjectImportHostedService> logger, IServiceProvider serviceProvider)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_projectParsers = new Dictionary<string, IProjectParser>();
            m_importManager = importManager;
            m_logger = logger;
            m_serviceProvider = serviceProvider;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ExternalRepositoryTypeName, manager);
            }

            foreach (var parser in parsers)
            {
                m_projectParsers.Add(parser.BibliographicFormatName, parser);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Project import hosted service started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var externalRepositories = await m_importManager.GetExternalRepositories(cancellationToken);

                var importTasks = new List<Task>();
                foreach (var externalRepository in externalRepositories)
                {
                    var cts = new CancellationTokenSource();
                    m_importManager.CancellationTokens.TryAdd(externalRepository.Id, cts);
                    importTasks.Add(Import(externalRepository, cts.Token));
                }

                await Task.WhenAll(importTasks);

                m_importManager.IsImportRunning = false;
                m_importManager.ActualProgress.Clear();
                m_importManager.CancellationTokens.Clear();
            }

            m_logger.LogInformation("Project import hosted service stopped.");
        }

        private async Task Import(ExternalRepository externalRepository, CancellationToken cancellationToken)
        {
            var progressInfo = new RepositoryImportProgressInfo(externalRepository.Id);
            m_importManager.ActualProgress.TryAdd(externalRepository.Id, progressInfo);

            try
            {
                ImportHistory importHistory;
                IDictionary<string, List<string>> filteringExpressions;

                using (var scope = m_serviceProvider.CreateScope())
                {
                    var importHistoryManager = scope.ServiceProvider.GetRequiredService<ImportHistoryManager>();
                    var importHistoryId = importHistoryManager.CreateImportHistory(externalRepository, m_importManager.UserId);
                    importHistory = importHistoryManager.GetImportHistory(importHistoryId);

                    var filteringExpressionSetManager = scope.ServiceProvider.GetRequiredService<FilteringExpressionSetManager>();
                    filteringExpressions =
                        filteringExpressionSetManager.GetFilteringExpressionsByExternalRepository(externalRepository.Id);
                }


                m_projectImportManagers.TryGetValue(externalRepository.ExternalRepositoryType.Name, out var importManager);
                if (importManager == null)
                {
                    throw new ArgumentNullException(
                        $"Import manager was not found for repository type {externalRepository.ExternalRepositoryType.Name}.");
                }

                var executionOptions = new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken};

                var responseParserBlock = new TransformBlock<object, ProjectImportMetadata>(
                    response => importManager.ParseResponse(response),
                    executionOptions
                );

                m_projectParsers.TryGetValue(externalRepository.BibliographicFormat.Name, out var parser);
                if (parser == null)
                {
                    throw new ArgumentNullException(
                        $"Project parser was not found for bibliographic format {externalRepository.BibliographicFormat.Name}.");
                }

                var filterBlock = new TransformBlock<ProjectImportMetadata, ProjectImportMetadata>(metadata =>
                    {
                        if (metadata.IsFaulted)
                        {
                            return metadata;
                        }

                        try
                        {
                            using (var scope = m_serviceProvider.CreateScope())
                            {
                                var importMetadataManager = scope.ServiceProvider.GetRequiredService<ImportMetadataManager>();
                                var metadataDb = importMetadataManager.GetImportMetadataByExternalId(metadata.ExternalId);
                                metadata.IsNew = metadataDb == null;

                                if (metadata.IsNew)
                                {
                                    foreach (var item in parser.GetListPairIdValue(metadata))
                                    {
                                        filteringExpressions.TryGetValue(item.Id, out var filterExpressions);
                                        if (filterExpressions == null)
                                        {
                                            continue;
                                        }

                                        foreach (var expression in filterExpressions)
                                        {
                                            var expr = Regex.Escape(expression);
                                            expr = expr.Replace("%", ".*");

                                            if (Regex.IsMatch(item.Value, expr))
                                            {
                                                metadata.IsSuitable = true;
                                                break;
                                            }
                                        }

                                        if (metadata.IsSuitable)
                                        {
                                            break;
                                        }
                                    }

                                    metadata.IsSuitable = false;
                                }
                                else
                                {
                                    metadata.ProjectId = metadataDb.Snapshot.Project.Id;
                                    metadata.IsSuitable = true;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            metadata.IsFaulted = true;
                            metadata.FaultedMessage = e.Message;
                        }

                        return metadata;
                    }, executionOptions
                );

                var projectParserBlock = new TransformBlock<ProjectImportMetadata, ProjectImportMetadata>(
                    projectImportMetadata => parser.Parse(projectImportMetadata),
                    executionOptions
                );


                var nullTargetBlock = new ActionBlock<ProjectImportMetadata>(
                    projectImportMetadata => { progressInfo.IncrementProcessedProjectsCount(); }, executionOptions
                );


                var userId = m_importManager.UserId;


                var saveBlock = new ActionBlock<ProjectImportMetadata>(projectImportMetadata =>
                    {
                        using (var scope = m_serviceProvider.CreateScope())
                        {
                            var projectManager = scope.ServiceProvider.GetRequiredService<ProjectManager>();
                            var importMetadataManager = scope.ServiceProvider.GetRequiredService<ImportMetadataManager>();

                            try
                            {
                                if (!projectImportMetadata.IsFaulted)
                                {
                                    if (projectImportMetadata.IsNew)
                                    {
                                        var projectId = projectManager.CreateProject(projectImportMetadata, userId);
                                        projectImportMetadata.ProjectId = projectId;
                                    }

                                    projectManager.CreateProjectMetadata(projectImportMetadata, userId);
                                }
                            }
                            catch (Exception e)
                            {
                                m_logger.LogWarning(e.Message);
                                projectImportMetadata.FaultedMessage = e.Message;
                            }

                            importMetadataManager.CreateImportMetadata(projectImportMetadata, importHistory);
                            progressInfo.IncrementProcessedProjectsCount();
                        }
                    },
                    new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken}
                );

                var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};

                var buffer = new BufferBlock<object>(executionOptions);
                buffer.LinkTo(responseParserBlock, linkOptions);

                responseParserBlock.LinkTo(filterBlock, linkOptions);
                filterBlock.LinkTo(projectParserBlock, linkOptions, projectMetadata => projectMetadata.IsSuitable);
                filterBlock.LinkTo(nullTargetBlock, linkOptions);
                projectParserBlock.LinkTo(saveBlock, linkOptions);

                await importManager.ImportFromResource(externalRepository.Configuration, buffer, progressInfo, cancellationToken);
                buffer.Complete();

                saveBlock.Completion.Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                progressInfo.FaultedMessage = $"Import from repository {externalRepository.Name} was canceled.";
            }
            catch (AggregateException e)
            {
                var errorMessages = string.Empty;
                var exceptions = e.InnerExceptions;
                for (var i = 1; i <= exceptions.Count; i++)
                {
                    errorMessages += $"Error #{i}: {exceptions[i].Message}";
                }

                var message =
                    $"Errors occurred executing import task (import from repository {externalRepository.Name}). Error messages: {errorMessages}";
                m_logger.LogWarning(message);
                progressInfo.FaultedMessage = message;
            }
            catch (Exception e)
            {
                var message =
                    $"Error occurred executing import task (import from repository {externalRepository.Name}). Error message: {e.Message}";
                m_logger.LogWarning(message);

                progressInfo.FaultedMessage = message;
            }
            finally
            {
                progressInfo.IsCompleted = true;
                //TODO save progressInfo to DB
            }
        }
    }
}