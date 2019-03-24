using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
using Vokabular.Shared.Extensions;

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

            using (var scope = m_serviceProvider.CreateScope())
            {
                var importHistoryManager = scope.ServiceProvider.GetRequiredService<ImportHistoryManager>();
                var latestImportHistory = importHistoryManager.GetLatestSuccessfulImportHistory(externalRepository.Id);

                var importHistoryId = importHistoryManager.CreateImportHistory(externalRepository, m_importManager.UserId);
                var importHistory = importHistoryManager.GetImportHistory(importHistoryId);

                ImportPipeline.ImportPipeline importPipeline = null;

                try
                {
                    m_projectImportManagers.TryGetValue(externalRepository.ExternalRepositoryType.Name, out var importManager);
                    if (importManager == null)
                    {
                        throw new ArgumentNullException(
                            $"Import manager was not found for repository type {externalRepository.ExternalRepositoryType.Name}.");
                    }


                    var builder = scope.ServiceProvider.GetRequiredService<ImportPipelineBuilder>();
                    importPipeline = builder.Build(externalRepository, importHistory, progressInfo, cancellationToken);

                    await importManager.ImportFromResource(externalRepository.Configuration, importPipeline.BufferBlock, progressInfo,
                        latestImportHistory?.Date, cancellationToken);
                    importPipeline.BufferBlock.Complete();

                    importPipeline.LastBlock.Completion.Wait(cancellationToken); //TODO check throw exception 
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

                    if(m_logger.IsErrorEnabled())
                        m_logger.LogError(e, message);

                    progressInfo.FaultedMessage = message;
                }
                catch (ImportFailedException e)
                {
                    var message =
                        $"Error occurred executing import task (import from repository {externalRepository.Name}). Error message: {e.Message}";

                    if (m_logger.IsErrorEnabled())
                        m_logger.LogError(e, message);

                    progressInfo.FaultedMessage = message;
                }
                catch (Exception e)
                {
                    var message =
                        $"Error occurred executing import task (import from repository {externalRepository.Name}). Error message: {e.Message}";
                    if (m_logger.IsErrorEnabled())
                        m_logger.LogError(e, message, null);

                    progressInfo.FaultedMessage = message;
                }
                finally
                {
                    importPipeline?.LastBlock.Completion.Wait(cancellationToken);
                    progressInfo.IsCompleted = true;

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

                    importHistoryManager.UpdateImportHistory(importHistory);
                }
            }
        }
    }
}