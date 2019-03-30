using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.Shared.Extensions;

namespace Vokabular.ProjectImport.ImportPipeline
{
    public class ImportPipelineManager
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly ImportManager m_importManager;
        private readonly ILogger<ImportPipelineManager> m_logger;
        private readonly ImportPipelineBuilder m_importPipelineBuilder;
        private readonly ExternalRepositoryManager m_externalRepositoryManager;
        private readonly ImportHistoryManager m_importHistoryManager;

        public ImportPipelineManager(IEnumerable<IProjectImportManager> importManagers, ImportManager importManager,
            ILogger<ImportPipelineManager> logger, ImportPipelineBuilder importPipelineBuilder,
            ExternalRepositoryManager externalRepositoryManager, ImportHistoryManager importHistoryManager)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_importManager = importManager;
            m_logger = logger;
            m_importPipelineBuilder = importPipelineBuilder;
            m_externalRepositoryManager = externalRepositoryManager;
            m_importHistoryManager = importHistoryManager;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ExternalRepositoryTypeName, manager);
            }
        }

        public async Task ImportAsync(int externalRepositoryId, CancellationToken cancellationToken)
        {
            var progressInfo = new RepositoryImportProgressInfo(externalRepositoryId);
            m_importManager.ActualProgress.TryAdd(externalRepositoryId, progressInfo);

            var latestImportHistory = m_importHistoryManager.GetLatestSuccessfulImportHistory(externalRepositoryId);

            var importHistoryId = m_importHistoryManager.CreateImportHistory(externalRepositoryId, m_importManager.UserId);

            var externalRepository = m_externalRepositoryManager.GetExternalRepository(externalRepositoryId);

            ImportPipeline importPipeline = null;

            try
            {
                m_projectImportManagers.TryGetValue(externalRepository.ExternalRepositoryType.Name, out var importManager);
                if (importManager == null)
                {
                    throw new ArgumentNullException(
                        $"Import manager was not found for repository type {externalRepository.ExternalRepositoryType.Name}.");
                }

                importPipeline = m_importPipelineBuilder.Build(externalRepository, importHistoryId, progressInfo, cancellationToken);

                await importManager.ImportFromResource(externalRepository.Configuration, importPipeline.BufferBlock, progressInfo,
                    latestImportHistory?.Date, cancellationToken);
                importPipeline.BufferBlock.Complete();

                importPipeline.LastBlock.Completion.Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                progressInfo.FaultedMessage = $"Import from repository {externalRepository.Name} was canceled.";
            }
            catch (AggregateException e)
            {
                var errorMessages = new StringBuilder();
                var exceptions = e.InnerExceptions;
                for (var i = 1; i <= exceptions.Count; i++)
                {
                    errorMessages.AppendLine($"Error #{i}: {exceptions[i].Message}");
                }

                if (m_logger.IsErrorEnabled())
                    m_logger.LogError(e, errorMessages.ToString());

                progressInfo.FaultedMessage = errorMessages.ToString();
            }
            catch (ImportFailedException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError(e, e.Message);

                progressInfo.FaultedMessage = e.Message;
            }
            catch (Exception e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError(e, e.Message);

                progressInfo.FaultedMessage = e.Message;
                throw;
            }
            finally
            {
                m_importManager.CancelTask(externalRepositoryId);
                progressInfo.IsCompleted = true;
                var importHistory = m_importHistoryManager.GetImportHistory(importHistoryId);

                if (!string.IsNullOrEmpty(progressInfo.FaultedMessage))
                {
                    progressInfo.FaultedMessage = $"Error occurred executing import task (import from repository {externalRepository.Name}). Error message: {progressInfo.FaultedMessage}";
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

                m_importHistoryManager.UpdateImportHistory(importHistory);
                importPipeline?.LastBlock.Completion.Wait(cancellationToken);
            }
        }
    }
}