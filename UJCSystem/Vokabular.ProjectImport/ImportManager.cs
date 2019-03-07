using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Model;

namespace Vokabular.ProjectImport
{
    public class ImportManager
    {
        private IList<ExternalRepository> m_importList;
        private readonly SemaphoreSlim m_signal;
        private readonly object m_updateListLock = new object();

        public ImportManager()
        {
            m_importList = new List<ExternalRepository>();
            m_signal = new SemaphoreSlim(0);
            ActualProgress = new ConcurrentDictionary<int, ProjectImportProgressInfo>();
            CancellationTokens = new ConcurrentDictionary<int, CancellationTokenSource>();
            IsImportRunning = false;
        }

        public readonly ConcurrentDictionary<int, ProjectImportProgressInfo> ActualProgress;
        public readonly ConcurrentDictionary<int, CancellationTokenSource> CancellationTokens;
        public bool IsImportRunning { get; private set; }
        public int UserId { get; private set; }

        public void ImportFromResources(IList<ExternalRepository> externalRepositories, int userId)
        {
            if (IsImportRunning)
            {
                //TODO check if import running -> exception
                throw new Exception();
            }

            if (externalRepositories == null || externalRepositories.Count == 0)
            {
                throw new ArgumentNullException(nameof(externalRepositories));
            }

            m_importList = externalRepositories;
            UserId = userId;
            m_signal.Release();
        }

        public void UpdateList(ProjectImportProgressInfo progressInfo)
        {
            lock (m_updateListLock)
            {
                //TODO own class in ConcDic instead of ProgressInfo
                ActualProgress.TryGetValue(progressInfo.ExternalRepositoryId, out var progress);
                ActualProgress.TryUpdate(progressInfo.ExternalRepositoryId, progressInfo, progress);
                IsImportRunning = ActualProgress.Any(x => !x.Value.IsCompleted);

                if (!IsImportRunning)
                {
                    ActualProgress.Clear();
                    CancellationTokens.Clear();
                }
            }
        }

        public async Task<IEnumerable<ExternalRepository>> GetExternalRepositories(CancellationToken cancellationToken)
        {
            await m_signal.WaitAsync(cancellationToken);
            IsImportRunning = true;
            lock (m_updateListLock)
            {
                foreach (var externalRepository in m_importList)
                {
                    ActualProgress.TryAdd(externalRepository.Id, new ProjectImportProgressInfo(externalRepository.Id));
                }
            }

            return m_importList;
        }
    }
}