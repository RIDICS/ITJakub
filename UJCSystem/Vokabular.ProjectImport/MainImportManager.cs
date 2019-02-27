using System.Collections.Generic;
using Vokabular.ProjectImport.DataEntities;
using Vokabular.ProjectImport.DataEntities.Database;

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

        public IReadOnlyDictionary<string, ProjectImportProgressInfo> ActualProgress => m_importManager.ActualProgress;
        public bool IsImportRunning => m_importManager.IsImportRunning;

        public void ImportFromResources(IList<Resource> resources)
        {
            m_importManager.ImportFromResources(resources);
        }

        public void CancelTask(string resourceName)
        {
            m_importManager.CancellationTokens.TryGetValue(resourceName, out var tokenSource);
            tokenSource?.Cancel();
        }
    }
}