using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.OaiPmhImportManager;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing;
using Vokabular.ProjectParsing.Parsers;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport
{
    public class ProjectImportHostedService : BackgroundService
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<string, IProjectParser> m_projectParsers;
        private readonly IDictionary<string, IProjectFilter> m_projectFilters;
        private readonly ImportManager m_importManager;
        private readonly ILogger<ProjectImportHostedService> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ProjectImportHostedService(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IProjectParser> parsers, IEnumerable<IProjectFilter> filters,
            ImportManager importManager, ILogger<ProjectImportHostedService> logger, IServiceProvider serviceProvider)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_projectParsers = new Dictionary<string, IProjectParser>();
            m_projectFilters = new Dictionary<string, IProjectFilter>();
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

            foreach (var filter in filters)
            {
                m_projectFilters.Add(filter.BibliographicFormatName, filter);
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
                    importTasks.Add(
                        Import(externalRepository, new Progress<RepositoryImportProgressInfo>(m_importManager.UpdateList), cts.Token));
                }

                await Task.WhenAll(importTasks);
            }

            m_logger.LogInformation("Project import hosted service stopped.");
        }

        private async Task Import(ExternalRepository externalRepository, IProgress<RepositoryImportProgressInfo> progress,
            CancellationToken cancellationToken)
        {
            var progressInfo = new RepositoryImportProgressInfo(externalRepository.Id);

            using (var scope = m_serviceProvider.CreateScope())
            {
                try
                {
                    //TODO move to ImportManager?
                    var importHistoryManager = scope.ServiceProvider.GetRequiredService<ImportHistoryManager>();
                    importHistoryManager.CreateImportHistory(externalRepository, m_importManager.UserId);

                    //TODO move to ???
                    var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhRepositoryConfiguration>(externalRepository.Configuration);
                    var config = new Dictionary<ParserHelperTypes, string> {{ParserHelperTypes.TemplateUrl, oaiPmhResource.TemplateUrl}};

                    m_projectImportManagers.TryGetValue(externalRepository.ExternalRepositoryType.Name, out var importManager);
                    if (importManager == null)
                    {
                        throw new ArgumentNullException($"Import manager was not found for repository type {externalRepository.ExternalRepositoryType.Name}.");
                    }

                    var executionOptions = new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken};

                    var responseParserBlock = new TransformBlock<object, ProjectImportMetadata>(
                        response => importManager.ParseResponse(response),
                        executionOptions
                    );


                    var filteringExpressionSetManager = scope.ServiceProvider.GetRequiredService<FilteringExpressionSetManager>();
                    var filteringExpressions = filteringExpressionSetManager.GetFilteringExpressionsByExternalRepository(externalRepository.Id);
           
                    m_projectFilters.TryGetValue(externalRepository.BibliographicFormat.Name, out var projectFilter);
                    if (projectFilter == null)
                    {
                        throw new ArgumentNullException($"Project filter was not found for bibliographic format {externalRepository.BibliographicFormat.Name}.");
                    }

                    var filterBlock = new TransformBlock<ProjectImportMetadata, ProjectImportMetadata>(
                        metadata => projectFilter.Filter(metadata, filteringExpressions),
                        executionOptions
                    );


                    m_projectParsers.TryGetValue(externalRepository.BibliographicFormat.Name, out var parser);
                    if (parser == null)
                    {
                        throw new ArgumentNullException($"Project parser was not found for bibliographic format {externalRepository.BibliographicFormat.Name}.");
                    }

                    var projectParserBlock = new TransformBlock<ProjectImportMetadata, ProjectImportMetadata>(
                        projectImportMetadata => parser.Parse(projectImportMetadata, config),
                        executionOptions
                    );


                    var nullTargetBlock = new ActionBlock<ProjectImportMetadata>(
                        projectImportMetadata =>
                        {
                            progressInfo.IncrementProcessedProjectsCount();
                            progress.Report(progressInfo);
                        }, executionOptions
                    );

                    var projectRepository = scope.ServiceProvider.GetRequiredService<ProjectRepository>();

                    var userId = m_importManager.UserId;

                    var saveBlock = new ActionBlock<ProjectImportMetadata>(projectImportMetadata =>
                        {
                            if (!projectImportMetadata.IsFaulted)
                            {
                                if (projectImportMetadata.IsNew)
                                {
                                    var projectId = new CreateImportedProjectWork(projectRepository, projectImportMetadata, userId).Execute();
                                }

                                new CreateImportedMetadataWork(projectRepository, projectImportMetadata, userId).Execute();
                            }

                            //TODO save to DB ProjectImportMetadata - to separated block

                            progressInfo.IncrementProcessedProjectsCount();
                            progress.Report(progressInfo);
                        }, new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken}
                    );

                    var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};

                    var buffer = new BufferBlock<object>(executionOptions);
                    buffer.LinkTo(responseParserBlock, linkOptions);

                    responseParserBlock.LinkTo(filterBlock, linkOptions);
                    filterBlock.LinkTo(projectParserBlock, linkOptions, projectMetadata => projectMetadata.IsSuitable);
                    filterBlock.LinkTo(nullTargetBlock, linkOptions);
                    projectParserBlock.LinkTo(saveBlock, linkOptions);

                    await importManager.ImportFromResource(externalRepository.Configuration, buffer, cancellationToken);
                    buffer.Complete();

                    saveBlock.Completion.Wait(cancellationToken);
                }
                catch (Exception e)
                {
                    var message = $"Error occurred executing import task (import from repository {externalRepository.Name}). Error message: {e.Message}";
                    if (!(e is OperationCanceledException))
                    {
                        m_logger.LogWarning(message);
                    }

                    progressInfo.FaultedMessage = message;
                }
                finally
                {
                    progressInfo.IsCompleted = true;
                    progress.Report(progressInfo);
                    //TODO count: updated, new, deleted - own entity? - save to DB (return from this method?)
                }
            }
        }
    }
}