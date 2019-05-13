using System.Collections.Generic;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;

namespace Vokabular.ProjectImport
{
    /// <summary>
    /// The entry point for starting import from external resources
    /// </summary>
    public class MainImportManager
    {
        private readonly ImportManager m_importManager;

        public MainImportManager(ImportManager importManager)
        {
            m_importManager = importManager;
        }

        public IReadOnlyDictionary<int, RepositoryImportProgressInfo> ActualProgress => m_importManager.ActualProgress;
        public bool IsImportRunning => m_importManager.IsImportRunning;

        /// <summary>
        /// Start importing from given external repositories.
        /// </summary>
        /// <param name="externalRepositoryIds"></param>
        /// <param name="userId">Id of the user who started the import</param>
        public virtual void ImportFromResources(IList<int> externalRepositoryIds, int userId)
        {
            m_importManager.ImportFromExternalRepositories(externalRepositoryIds, userId);
        }

        /// <summary>
        /// Cancel task from external repository.
        /// </summary>
        /// <param name="externalRepositoryId"></param>
        public void CancelTask(int externalRepositoryId)
        {
            m_importManager.CancelTask(externalRepositoryId);
        }
    }
}