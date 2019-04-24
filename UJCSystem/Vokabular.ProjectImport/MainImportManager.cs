using System.Collections.Generic;
using Vokabular.ProjectImport.Managers;
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

        public virtual void ImportFromResources(IList<int> externalRepositoryIds, int userId)
        {
            m_importManager.ImportFromExternalRepositories(externalRepositoryIds, userId);
        }

        public void CancelTask(int externalRepositoryId)
        {
            m_importManager.CancelTask(externalRepositoryId);
        }
    }
}