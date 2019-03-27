using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;

namespace Vokabular.ProjectImport.Managers
{
    public class ImportManager
    {
        private IList<int> m_importList;
        private readonly SemaphoreSlim m_signal;

        public ImportManager()
        {
            m_importList = new List<int>();
            m_signal = new SemaphoreSlim(0);
            ActualProgress = new ConcurrentDictionary<int, RepositoryImportProgressInfo>();
            CancellationTokens = new ConcurrentDictionary<int, CancellationTokenSource>();
            IsImportRunning = false;
        }

        public ConcurrentDictionary<int, RepositoryImportProgressInfo> ActualProgress { get; }
        public ConcurrentDictionary<int, CancellationTokenSource> CancellationTokens { get; }
        public bool IsImportRunning { get; set; }
        public int UserId { get; private set; }

        public void ImportFromResources(IList<int> externalRepositoryIds, int userId)
        {
            if (IsImportRunning)
            {
                throw new ImportRunningException();
            }

            if (externalRepositoryIds == null || externalRepositoryIds.Count == 0)
            {
                throw new ArgumentNullException(nameof(externalRepositoryIds));
            }

            m_importList.Clear();
            m_importList = externalRepositoryIds;
            UserId = userId;

            m_signal.Release();
        }

        public async Task<IEnumerable<int>> GetExternalRepositories(CancellationToken cancellationToken)
        {
            await m_signal.WaitAsync(cancellationToken);
            IsImportRunning = true;

            return m_importList;
        }
    }
}