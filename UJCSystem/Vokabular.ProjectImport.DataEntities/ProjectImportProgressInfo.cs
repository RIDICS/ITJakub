using System.Threading;

namespace Vokabular.ProjectImport.DataEntities
{
    public class ProjectImportProgressInfo
    {
        public int TotalProjectsCount { get; set; }
        public string ResourceName { get; }
        public bool IsCompleted { get; set; }
        public string FaultedMessage { get; set; }

        private int m_unusedProjectsCount;
        public int UnusedProjectsCount => m_unusedProjectsCount;
        public int IncrementUnusedProjectsCount() { return Interlocked.Increment(ref m_unusedProjectsCount); }

        private int m_updatedProjectsCount;
        public int UpdatedProjectsCount => m_updatedProjectsCount;
        public int IncrementUpdatedProjectsCount() { return Interlocked.Increment(ref m_updatedProjectsCount); }

        private int m_newProjectsCount;
        public int NewProjectsCount => m_newProjectsCount;
        public int IncrementNewProjectsCount() { return Interlocked.Increment(ref m_newProjectsCount); }

        public ProjectImportProgressInfo(string resourceName)
        {
            ResourceName = resourceName;
            m_unusedProjectsCount = 0;
            m_updatedProjectsCount = 0;
            m_newProjectsCount = 0;
            //TODO remove
            TotalProjectsCount = 5000;
        }
    }
}
