using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectParsing.Parsers;
using Project = Vokabular.ProjectParsing.Model.Entities.Project;

namespace Vokabular.ProjectImport
{
    public class ProjectImportHostedService : BackgroundService
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<string, IParser> m_parsers;
        private readonly ImportManager m_importManager;
        private readonly ILogger<ProjectImportHostedService> m_logger;

        public ProjectImportHostedService(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IParser> parsers,
            ImportManager importManager, ILogger<ProjectImportHostedService> logger)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_parsers = new Dictionary<string, IParser>();
            m_importManager = importManager;
            m_logger = logger;

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
                    m_importManager.CancellationTokens.TryAdd(externalResource.Name, cts);
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
            var progressInfo = new ProjectImportProgressInfo(externalResource.Name);

            try
            {
                var buffer = new BufferBlock<string>(new DataflowBlockOptions {CancellationToken = cancellationToken});

                var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhResource>(externalResource.Configuration);
                var config = new Dictionary<ParserHelperTypes, string> {{ParserHelperTypes.TemplateUrl, oaiPmhResource.TemplateUrl}};
                m_parsers.TryGetValue(externalResource.ParserType.Name, out var parser);

                if (parser == null)
                {
                    throw new NotImplementedException($"Parser manager was not found for resource {externalResource.Name}.");
                }

                var parserBlock = new TransformBlock<string, Project>(
                    xml => parser.Parse(xml, config), new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken}
                );

                var saveBlock = new ActionBlock<Project>(project =>
                    {
                        //TODO save to DB
                        progressInfo.IncrementNewProjectsCount();
                        progress.Report(progressInfo);
                    }, new ExecutionDataflowBlockOptions {CancellationToken = cancellationToken}
                );

                buffer.LinkTo(parserBlock, new DataflowLinkOptions {PropagateCompletion = true});
                parserBlock.LinkTo(saveBlock, new DataflowLinkOptions {PropagateCompletion = true});

                m_projectImportManagers.TryGetValue(externalResource.ExternalResourceType.Name, out var importManager);

                if (importManager == null)
                {
                    throw new NotImplementedException($"Import manager was not found for resource {externalResource.Name}.");
                }

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