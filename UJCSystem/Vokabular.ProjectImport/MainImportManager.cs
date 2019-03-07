using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Model;

namespace Vokabular.ProjectImport
{
    public class MainImportManager
    {
        private readonly ImportManager m_importManager;

        public MainImportManager(ImportManager importManager)
        {
            m_importManager = importManager;
        }

        public IReadOnlyDictionary<int, RepositoryImportProgressInfo> ActualProgress => m_importManager.ActualProgress;
        public bool IsImportRunning => m_importManager.IsImportRunning;

        public void ImportFromResources(IList<ExternalRepository> externalRepositories, int userId)
        {
            m_importManager.ImportFromResources(externalRepositories, userId);
        }

        public void CancelTask(int externalRepositoryId)
        {
            m_importManager.CancellationTokens.TryGetValue(externalRepositoryId, out var tokenSource);
            tokenSource?.Cancel();
        }
    }
}