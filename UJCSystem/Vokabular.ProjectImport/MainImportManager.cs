using System.Collections.Generic;
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

        public IReadOnlyDictionary<string, ProjectImportProgressInfo> ActualProgress => m_importManager.ActualProgress;
        public bool IsImportRunning => m_importManager.IsImportRunning;

        public void ImportFromResources(IList<int> externalResourcesId)
        {
            m_importManager.ImportFromResources(externalResourcesId);
        }

        //TODO change to Id?
        public void CancelTask(string resourceName)
        {
            m_importManager.CancellationTokens.TryGetValue(resourceName, out var tokenSource);
            tokenSource?.Cancel();
        }
    }
}