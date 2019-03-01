using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ProjectImport.Model;

namespace Vokabular.ProjectImport
{
    public class ImportManager
    {
        private readonly IServiceScopeFactory m_serviceScopeFactory;
        private readonly IList<ExternalResource> m_importList;
        private readonly SemaphoreSlim m_signal;
        private readonly object m_updateListLock = new object();

        public ImportManager(IServiceScopeFactory serviceScopeFactory)
        {
            m_serviceScopeFactory = serviceScopeFactory;
            m_importList = new List<ExternalResource>();
            m_signal = new SemaphoreSlim(0);
            ActualProgress = new ConcurrentDictionary<string, ProjectImportProgressInfo>();
            CancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource>();
            IsImportRunning = false;
        }

        public readonly ConcurrentDictionary<string, ProjectImportProgressInfo> ActualProgress;
        public readonly ConcurrentDictionary<string, CancellationTokenSource> CancellationTokens;
        public bool IsImportRunning { get; private set; }

        public void ImportFromResources(IList<int> externalResourcesId)
        {
            if (IsImportRunning)
            {
                //TODO check if import running -> exception
                throw new Exception();
            }

            if (externalResourcesId == null || externalResourcesId.Count == 0)
            {
                throw new ArgumentNullException(nameof(externalResourcesId));
            }

            m_importList.Clear();

            using (var scope = m_serviceScopeFactory.CreateScope())
            {
                var externalResourceRepository = scope.ServiceProvider.GetRequiredService<ExternalResourceRepository>();
                foreach (var id in externalResourcesId)
                {
                    var externalResource = externalResourceRepository.GetExternalResource(id);
                    m_importList.Add(externalResource);
                }
            }

            //TODO count updated, new, deleted items
            m_signal.Release();
        }

        public void UpdateList(ProjectImportProgressInfo progressInfo)
        {
            lock (m_updateListLock)
            {
                //TODO own class in ConcDic instead of ProgressInfo
                ActualProgress.TryGetValue(progressInfo.ResourceName, out var progress);
                ActualProgress.TryUpdate(progressInfo.ResourceName, progressInfo, progress);
                IsImportRunning = ActualProgress.Any(x => !x.Value.IsCompleted);

                if (!IsImportRunning)
                {
                    ActualProgress.Clear();
                    CancellationTokens.Clear();
                    //What now? save info to DB?
                    //TODO how to set timeout for 
                }
            }
        }

        public async Task<IEnumerable<ExternalResource>> GetExternalResources(CancellationToken cancellationToken)
        {
            await m_signal.WaitAsync(cancellationToken);
            IsImportRunning = true;
            lock (m_updateListLock)
            {
                foreach (var externalResource in m_importList)
                {
                    ActualProgress.TryAdd(externalResource.Name, new ProjectImportProgressInfo(externalResource.Name));
                }
            }

            return m_importList;
        }
    }
}