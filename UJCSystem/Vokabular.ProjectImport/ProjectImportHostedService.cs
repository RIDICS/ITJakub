using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.ProjectImport.ImportPipeline;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exception;
using Vokabular.ProjectParsing.Model.Parsers;
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

            ImportHistory importHistory;
            ImportHistory latestImportHistory;

            using (var scope = m_serviceProvider.CreateScope())
            {
                var importHistoryManager = scope.ServiceProvider.GetRequiredService<ImportHistoryManager>();
                latestImportHistory = importHistoryManager.GetLatestSuccessfulImportHistory(externalRepository.Id);

                var importHistoryId = importHistoryManager.CreateImportHistory(externalRepository, m_importManager.UserId);
                importHistory = importHistoryManager.GetImportHistory(importHistoryId);
            }


            try
            {
                m_projectImportManagers.TryGetValue(externalRepository.ExternalRepositoryType.Name, out var importManager);
                if (importManager == null)
                {
                    throw new ArgumentNullException(
                        $"Import manager was not found for repository type {externalRepository.ExternalRepositoryType.Name}.");
                }

                using (var scope = m_serviceProvider.CreateScope())
                {
                    var builder = scope.ServiceProvider.GetRequiredService<ImportPipelineBuilder>();
                    var importPipeline = builder.Build(externalRepository, importHistory, progressInfo, cancellationToken);

                    await importManager.ImportFromResource(externalRepository.Configuration, importPipeline.BufferBlock, progressInfo,
                        latestImportHistory?.Date, cancellationToken);
                    importPipeline.BufferBlock.Complete();

                    importPipeline.LastBlock.Completion.Wait(cancellationToken);
                }
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
            catch (System.Exception e) //TODO remove
            {
                var message =
                    $"Error occurred executing import task (import from repository {externalRepository.Name}). Error message: {e.Message}";
                m_logger.LogWarning(message);

                progressInfo.FaultedMessage = message;
            }
            finally
            {
                progressInfo.IsCompleted = true;
                using (var scope = m_serviceProvider.CreateScope())
                {
                    if (!string.IsNullOrEmpty(progressInfo.FaultedMessage))
                    {
                        importHistory.Message = progressInfo.FaultedMessage;
                        importHistory.Status = ImportStatusEnum.Failed;
                    }
                    else if (progressInfo.FailedProjectsCount > 0)
                    {
                        importHistory.Status = ImportStatusEnum.CompletedWithWarnings;
                    }
                    else
                    {
                        importHistory.Status = ImportStatusEnum.Completed;
                    }

                    var importHistoryManager = scope.ServiceProvider.GetRequiredService<ImportHistoryManager>();
                    importHistoryManager.UpdateImportHistory(importHistory);
                }
            }
        }
    }
}