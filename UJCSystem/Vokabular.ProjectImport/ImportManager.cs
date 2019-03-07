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
        private readonly IServiceProvider m_serviceProvider;
        private readonly IList<ExternalRepository> m_importList;
        private readonly SemaphoreSlim m_signal;
        private readonly object m_updateListLock = new object();

        public ImportManager(IServiceProvider serviceProvider)
        {
            m_serviceProvider = serviceProvider;
            m_importList = new List<ExternalRepository>();
            m_signal = new SemaphoreSlim(0);
            ActualProgress = new ConcurrentDictionary<int, RepositoryImportProgressInfo>();
            CancellationTokens = new ConcurrentDictionary<int, CancellationTokenSource>();
            IsImportRunning = false;
        }

        public readonly ConcurrentDictionary<int, RepositoryImportProgressInfo> ActualProgress;
        public readonly ConcurrentDictionary<int, CancellationTokenSource> CancellationTokens;
        public bool IsImportRunning { get; private set; }
        public int UserId { get; private set; }

        public void ImportFromResources(IList<int> externalRepositoryIds, int userId)
        {
            if (IsImportRunning)
            {
                //TODO check if import running -> exception
                throw new Exception();
            }

            if (externalRepositoryIds == null || externalRepositoryIds.Count == 0)
            {
                throw new ArgumentNullException(nameof(externalRepositoryIds));
            }

            m_importList.Clear();

            using (var scope = m_serviceProvider.CreateScope())
            {
                foreach (var externalRepositoryId in externalRepositoryIds)
                {
                    var externalRepositoryRepository = scope.ServiceProvider.GetRequiredService<ExternalRepositoryRepository>();
                    m_importList.Add(externalRepositoryRepository.GetExternalRepository(externalRepositoryId));
                }
            }
               
            UserId = userId;
            m_signal.Release();
        }

        public void UpdateList(RepositoryImportProgressInfo progressInfo)
        {
            lock (m_updateListLock)
            {
                //TODO own class in ConcDic instead of ProgressInfo
                if (ActualProgress.All(x => x.Value.IsCompleted))
                {
                    IsImportRunning = false;
                    ActualProgress.Clear();
                    CancellationTokens.Clear();
                }
            }
        }

        public async Task<IEnumerable<ExternalRepository>> GetExternalRepositories(CancellationToken cancellationToken)
        {
            await m_signal.WaitAsync(cancellationToken);
            IsImportRunning = true;

            foreach (var externalRepository in m_importList)
            {
                ActualProgress.TryAdd(externalRepository.Id, new RepositoryImportProgressInfo(externalRepository.Id));
            }

            return m_importList;
        }
    }
}