﻿using System;
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
        private IList<ExternalResource> m_importList;
        private readonly SemaphoreSlim m_signal;
        private readonly object m_updateListLock = new object();

        public ImportManager()
        {
            m_importList = new List<ExternalResource>();
            m_signal = new SemaphoreSlim(0);
            ActualProgress = new ConcurrentDictionary<string, ProjectImportProgressInfo>();
            CancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource>();
            IsImportRunning = false;
        }

        public readonly ConcurrentDictionary<string, ProjectImportProgressInfo> ActualProgress;
        public readonly ConcurrentDictionary<string, CancellationTokenSource> CancellationTokens;
        public bool IsImportRunning { get; private set; }

        public void ImportFromResources(IList<ExternalResource> externalResources)
        {
            if (IsImportRunning)
            {
                //TODO check if import running -> exception
                throw new Exception();
            }

            if (externalResources == null || externalResources.Count == 0)
            {
                throw new ArgumentNullException(nameof(externalResources));
            }

            m_importList = externalResources;
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