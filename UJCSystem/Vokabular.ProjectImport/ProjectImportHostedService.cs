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
using Vokabular.ProjectImport.ImportManagers;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Parsers;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport
{
    public class ProjectImportHostedService : BackgroundService
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<string, IParser> m_parsers;
        private readonly ImportManager m_importManager;
        private readonly ILogger<ProjectImportHostedService> m_logger;
        private readonly IServiceScopeFactory m_serviceScopeFactory;

        public ProjectImportHostedService(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IParser> parsers,
            ImportManager importManager, ILogger<ProjectImportHostedService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_parsers = new Dictionary<string, IParser>();
            m_importManager = importManager;
            m_logger = logger;
            m_serviceScopeFactory = serviceScopeFactory;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ExternalResourceTypeName, manager);
            }

            foreach (var parser in parsers)
            {
                m_parsers.Add(parser.ParserTypeName, parser);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Project import hosted service started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var externalResources = await m_importManager.GetExternalResources(cancellationToken);

                var importTasks = new List<Task>();
                foreach (var externalResource in externalResources)
                {
                    var cts = new CancellationTokenSource();
                    m_importManager.CancellationTokens.TryAdd(externalResource.Id, cts);
                    importTasks.Add(
                        Import(externalResource, new Progress<ProjectImportProgressInfo>(m_importManager.UpdateList), cts.Token));
                }

                await Task.WhenAll(importTasks);
            }

            m_logger.LogInformation("Project import hosted service stopped.");
        }

        private async Task Import(ExternalResource externalResource, IProgress<ProjectImportProgressInfo> progress,
            CancellationToken cancellationToken)
        {
            var progressInfo = new ProjectImportProgressInfo(externalResource.Id);

            using (var scope = m_serviceScopeFactory.CreateScope())
            {
                try
                {
                    //TODO move to MainService?
                    var importHistoryManager = scope.ServiceProvider.GetRequiredService<ImportHistoryManager>();
                    importHistoryManager.CreateImportHistory(externalResource, m_importManager.UserId);

                    var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhResource>(externalResource.Configuration);
                    var config = new Dictionary<ParserHelperTypes, string> {{ParserHelperTypes.TemplateUrl, oaiPmhResource.TemplateUrl}};

                    m_projectImportManagers.TryGetValue(externalResource.ExternalResourceType.Name, out var importManager);
                    if (importManager == null)
                    {
                        throw new ArgumentNullException($"Import manager was not found for resource {externalResource.Name}.");
                    }

                    var responseParserBlock = new TransformBlock<object, ProjectImportMetadata>(
                        response => importManager.ParseResponse(response),
                        new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken}
                    );

                    m_parsers.TryGetValue(externalResource.ParserType.Name, out var parser);
                    if (parser == null)
                    {
                        throw new ArgumentNullException($"Parser manager was not found for resource {externalResource.Name}.");
                    }

                    var projectParserBlock = new TransformBlock<ProjectImportMetadata, ProjectImportMetadata>(
                        projectImportMetadata => parser.Parse(projectImportMetadata, config),
                        new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken}
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

                    var buffer = new BufferBlock<string>(new DataflowBlockOptions {CancellationToken = cancellationToken});
                    buffer.LinkTo(responseParserBlock, new DataflowLinkOptions {PropagateCompletion = true});
                    responseParserBlock.LinkTo(projectParserBlock, new DataflowLinkOptions {PropagateCompletion = true});
                    projectParserBlock.LinkTo(saveBlock, new DataflowLinkOptions {PropagateCompletion = true});

                    await importManager.ImportFromResource(externalResource, buffer, cancellationToken);
                    buffer.Complete();

                    saveBlock.Completion.Wait(cancellationToken);
                }
                catch (Exception e)
                {
                    var message = $"Error occurred executing import task. Error message: {e.Message}";
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