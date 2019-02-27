using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vokabular.ProjectImport.DataEntities;
using Vokabular.ProjectImport.DataEntities.Database;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectParsing;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Parsers;

namespace Vokabular.ProjectImport
{
    public class ProjectImportHostedService : BackgroundService
    {
        private readonly IDictionary<ResourceType, IProjectImportManager> m_projectImportManagers;
        private readonly IDictionary<ParserType, IParser> m_parsers;
        private readonly ImportManager m_importManager;
        private readonly ILogger<ProjectImportHostedService> m_logger;

        public ProjectImportHostedService(IEnumerable<IProjectImportManager> importManagers, IEnumerable<IParser> parsers,
            ImportManager importManager, ILogger<ProjectImportHostedService> logger)
        {
            m_projectImportManagers = new Dictionary<ResourceType, IProjectImportManager>();
            m_parsers = new Dictionary<ParserType, IParser>();
            m_importManager = importManager;
            m_logger = logger;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ResourceType, manager);
            }

            foreach (var parser in parsers)
            {
                m_parsers.Add(parser.ParserType, parser);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Project import hosted service started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var resources = await m_importManager.GetResources(cancellationToken);

                var importTasks = new List<Task>();
                foreach (var resource in resources)
                {
                    var cts = new CancellationTokenSource();
                    m_importManager.CancellationTokens.TryAdd(resource.Name, cts);
                    importTasks.Add(Import(resource, new Progress<ProjectImportProgressInfo>(m_importManager.UpdateList), cts.Token));
                }

                await Task.WhenAll(importTasks);
            }

            m_logger.LogInformation("Project import hosted service stopped.");
        }

        private async Task Import(Resource resource, IProgress<ProjectImportProgressInfo> progress, CancellationToken cancellationToken)
        {
            var progressInfo = new ProjectImportProgressInfo(resource.Name);

            try
            {
                var buffer = new BufferBlock<string>(new DataflowBlockOptions {CancellationToken = cancellationToken});

                var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhResource>(resource.Configuration);
                var config = new Dictionary<ParserHelperTypes, string> {{ParserHelperTypes.TemplateUrl, oaiPmhResource.TemplateUrl}};
                m_parsers.TryGetValue(resource.ParserType, out var parser);

                if (parser == null)
                {
                    throw new NotImplementedException($"Parser manager was not found for resource {resource.Name}.");
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

                m_projectImportManagers.TryGetValue(resource.ResourceType, out var importManager);

                if (importManager == null)
                {
                    throw new NotImplementedException($"Import manager was not found for resource {resource.Name}.");
                }

                await importManager.ImportFromResource(resource, buffer, cancellationToken);
                buffer.Complete();

                saveBlock.Completion.Wait(cancellationToken);
            }
            catch (Exception e)
            {
                if (!(e is OperationCanceledException))
                {
                    m_logger.LogWarning($"Error occurred executing import task. Error message: {e.Message}");
                }
                
                progressInfo.FaultedMessage = e.Message;
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