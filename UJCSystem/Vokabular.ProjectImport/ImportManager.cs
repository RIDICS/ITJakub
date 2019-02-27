using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vokabular.ProjectImport.DataEntities;
using Vokabular.ProjectImport.DataEntities.Database;

namespace Vokabular.ProjectImport
{
    public class ImportManager
    {
        private IList<Resource> m_importList;
        private readonly SemaphoreSlim m_signal;
        private readonly object m_updateListLock = new object();

        public ImportManager()
        {
            m_signal = new SemaphoreSlim(0);
            ActualProgress = new ConcurrentDictionary<string, ProjectImportProgressInfo>();
            CancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource>();
            IsImportRunning = false;
        }

        public readonly ConcurrentDictionary<string, ProjectImportProgressInfo> ActualProgress;
        public readonly ConcurrentDictionary<string, CancellationTokenSource> CancellationTokens;
        public bool IsImportRunning { get; private set; }

        public void ImportFromResources(IList<Resource> resources)
        {
            if (IsImportRunning)
            {
                //TODO check if import running -> exception
                throw new Exception();
            }

            if (resources == null || resources.Count == 0)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            m_importList = resources;

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

        public async Task<IEnumerable<Resource>> GetResources(CancellationToken cancellationToken)
        {
            await m_signal.WaitAsync(cancellationToken);
            IsImportRunning = true;
            lock (m_updateListLock)
            {
                foreach (var resource in m_importList)
                {
                    ActualProgress.TryAdd(resource.Name, new ProjectImportProgressInfo(resource.Name));
                }
            }

            return m_importList;
        }
    }
}