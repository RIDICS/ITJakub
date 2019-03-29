using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectImport.ImportPipeline;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.Shared.Extensions;

namespace Vokabular.ProjectImport
{
    public class ProjectImportHostedService : BackgroundService
    {
        private readonly IDictionary<string, IProjectImportManager> m_projectImportManagers;
        private readonly ImportManager m_importManager;
        private readonly ILogger<ProjectImportHostedService> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ProjectImportHostedService(IEnumerable<IProjectImportManager> importManagers, ImportManager importManager,
            ILogger<ProjectImportHostedService> logger, IServiceProvider serviceProvider)
        {
            m_projectImportManagers = new Dictionary<string, IProjectImportManager>();
            m_importManager = importManager;
            m_logger = logger;
            m_serviceProvider = serviceProvider;

            foreach (var manager in importManagers)
            {
                m_projectImportManagers.Add(manager.ExternalRepositoryTypeName, manager);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Project import hosted service started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var externalRepositories = await m_importManager.GetExternalRepositories(cancellationToken);

                var importTasks = new List<Task>();
                using (var scope = m_serviceProvider.CreateScope())
                {
                    var importPipelineManager = scope.ServiceProvider.GetRequiredService<ImportPipelineManager>();
                    foreach (var externalRepositoryId in externalRepositories)
                    {
                        var cts = new CancellationTokenSource();
                        m_importManager.CancellationTokens.TryAdd(externalRepositoryId, cts);
                        importTasks.Add(importPipelineManager.ImportAsync(externalRepositoryId, cts.Token));
                    }
                    //TODO catch exceptions?
                    await Task.WhenAll(importTasks);
                }

                //TODO to finally 
                m_importManager.IsImportRunning = false;
                m_importManager.ActualProgress.Clear();
                m_importManager.CancellationTokens.Clear();
            }

            m_logger.LogInformation("Project import hosted service stopped.");
        }
    }
}