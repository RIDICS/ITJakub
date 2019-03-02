using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Model;

namespace Vokabular.ProjectImport
{
    public class MainImportManager
    {
        private readonly ImportManager m_importManager;
        //TODO set who starts importing

        public MainImportManager(ImportManager importManager)
        {
            m_importManager = importManager;
        }

        public IReadOnlyDictionary<int, ProjectImportProgressInfo> ActualProgress => m_importManager.ActualProgress;
        public bool IsImportRunning => m_importManager.IsImportRunning;

        public void ImportFromResources(IList<ExternalResource> externalResources)
        {
            m_importManager.ImportFromResources(externalResources);
        }

        public void CancelTask(int externalResourceId)
        {
            m_importManager.CancellationTokens.TryGetValue(externalResourceId, out var tokenSource);
            tokenSource?.Cancel();
        }
    }
}