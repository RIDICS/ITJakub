using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts;
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
        private readonly ImportPipelineDirector m_importPipelineDirector;
        private readonly ExternalRepositoryManager m_externalRepositoryManager;
        private readonly ImportHistoryManager m_importHistoryManager;

        public ImportPipelineManager(IEnumerable<IProjectImportManager> importManagers, ImportManager importManager,
            ILogger<ImportPipelineManager> logger, ImportPipelineBuilder importPipelineBuilder,
            ImportPipelineDirector importPipelineDirector, ExternalRepositoryManager externalRepositoryManager,
            ImportHistoryManager importHistoryManager)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_importManager = importManager;
            m_logger = logger;
            m_importPipelineBuilder = importPipelineBuilder;
            m_importPipelineDirector = importPipelineDirector;
            m_externalRepositoryManager = externalRepositoryManager;
            m_importHistoryManager = importHistoryManager;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ExternalRepositoryTypeName, manager);
            }
        }

        public async Task ImportAsync(int externalRepositoryId, CancellationToken cancellationToken)
        {
            var externalRepository = m_externalRepositoryManager.GetExternalRepository(externalRepositoryId);
            var progressInfo = new RepositoryImportProgressInfo(externalRepositoryId, externalRepository.Name);
            m_importManager.ActualProgress.TryAdd(externalRepositoryId, progressInfo);

            var latestImportHistory = m_importHistoryManager.GetLatestSuccessfulImportHistory(externalRepositoryId);

            var importHistoryId = m_importHistoryManager.CreateImportHistory(externalRepositoryId, m_importManager.UserId);


            ImportPipeline importPipeline = null;

            try
            {
                m_projectImportManagers.TryGetValue(externalRepository.ExternalRepositoryType.Name, out var importManager);
                if (importManager == null)
                {
                    throw new ImportFailedException(MainServiceErrorCode.RepositoryImportManagerNotFound,
                        $"The import manager was not found for repository type {externalRepository.ExternalRepositoryType.Name}.",
                        externalRepository.ExternalRepositoryType.Name);
                }

                importPipeline = m_importPipelineDirector.Build(m_importPipelineBuilder, externalRepository, importHistoryId, progressInfo, cancellationToken);

                await importManager.ImportFromResource(externalRepository.Configuration, importPipeline.BufferBlock, progressInfo,
                    latestImportHistory?.Date, cancellationToken);
                importPipeline.BufferBlock.Complete();

                importPipeline.LastBlock.Completion.Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                progressInfo.FaultedMessage = MainServiceErrorCode.RepositoryImportCancelled;
                progressInfo.FaultedMessageParams = new object[]{externalRepository.Name};
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

                progressInfo.FaultedMessage = MainServiceErrorCode.RepositoryImportFailed;
                progressInfo.FaultedMessageParams = new object[] { externalRepository.Name };
            }
            catch (ImportFailedException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError(e, e.Message);

                progressInfo.FaultedMessage = e.Code;
                progressInfo.FaultedMessageParams = e.CodeParams;
            }
            catch (Exception e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError(e, e.Message);

                progressInfo.FaultedMessage = MainServiceErrorCode.RepositoryImportFailed;
                progressInfo.FaultedMessageParams = new object[] { externalRepository.Name };
                throw;
            }
            finally
            {
                var importHistory = m_importHistoryManager.GetImportHistory(importHistoryId);
                m_importManager.CancellationTokens.TryGetValue(externalRepositoryId, out var cancellationTokenSource);

                if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                {
                    progressInfo.FaultedMessage = MainServiceErrorCode.RepositoryImportCancelled;
                    progressInfo.FaultedMessageParams = new object[] { externalRepository.Name };
                    importHistory.Message = progressInfo.FaultedMessage;
                    importHistory.Status = ImportStatusEnum.Failed;
                }
                else if (!string.IsNullOrEmpty(progressInfo.FaultedMessage))
                {
                    importHistory.Message = progressInfo.FaultedMessage;
                    importHistory.Status = ImportStatusEnum.Failed;
                    m_importManager.CancelTask(externalRepositoryId);
                }
                else if (progressInfo.FailedProjectsCount > 0)
                {
                    importHistory.Status = ImportStatusEnum.CompletedWithWarnings;
                }
                else
                {
                    importHistory.Status = ImportStatusEnum.Completed;
                }

                progressInfo.IsCompleted = true;
                m_importHistoryManager.UpdateImportHistory(importHistory);

                if (!string.IsNullOrEmpty(progressInfo.FaultedMessage))
                {
                    importPipeline?.LastBlock.Completion.Wait(cancellationToken);
                }
            }
        }
    }
}