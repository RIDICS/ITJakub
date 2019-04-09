﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vokabular.ProjectImport.ImportPipeline;
using Vokabular.ProjectImport.Managers;
using Vokabular.Shared.Extensions;

namespace Vokabular.ProjectImport
{
    public class ProjectImportHostedService : BackgroundService
    {
        private readonly ImportManager m_importManager;
        private readonly ILogger<ProjectImportHostedService> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ProjectImportHostedService(ImportManager importManager, ILogger<ProjectImportHostedService> logger, IServiceProvider serviceProvider)
        {
            m_importManager = importManager;
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Project import hosted service started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var externalRepositories = await m_importManager.GetExternalRepositories(cancellationToken);

                var importTasks = new List<Task>();
                try
                {
                    using (var scope = m_serviceProvider.CreateScope())
                    {
                        var importPipelineManager = scope.ServiceProvider.GetRequiredService<ImportPipelineManager>();
                        foreach (var externalRepositoryId in externalRepositories)
                        {
                            var cts = new CancellationTokenSource();
                            m_importManager.CancellationTokens.TryAdd(externalRepositoryId, cts);
                            importTasks.Add(importPipelineManager.ImportAsync(externalRepositoryId, cts.Token));
                        }

                        await Task.WhenAll(importTasks);
                    }
                }
                catch (OperationCanceledException e)
                {
                    if (m_logger.IsErrorEnabled())
                        m_logger.LogError(e, e.Message);
                }
                catch (Exception e)
                {
                    if (m_logger.IsErrorEnabled())
                        m_logger.LogError(e, e.Message);
                }
                finally
                {
                    foreach (var cancellationTokenSource in m_importManager.CancellationTokens)
                    {
                        cancellationTokenSource.Value.Cancel();
                    }
                    m_importManager.IsImportRunning = false;
                }
            }

            m_logger.LogInformation("Project import hosted service stopped.");
        }
    }
}