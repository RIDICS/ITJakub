using System.Threading;

namespace Vokabular.ProjectImport.Model
{
    public class RepositoryImportProgressInfo
    {
        private int m_processedProjectsCount;

        public RepositoryImportProgressInfo(int externalRepositoryId)
        {
            ExternalRepositoryId = externalRepositoryId;
            m_processedProjectsCount = 0;
        }

        public int TotalProjectsCount { get; set; }
        public int ExternalRepositoryId { get; }
        public bool IsCompleted { get; set; }
        public string FaultedMessage { get; set; }


        public int ProcessedProjectsCount => m_processedProjectsCount;
        public int IncrementProcessedProjectsCount() { return Interlocked.Increment(ref m_processedProjectsCount); }
    }
}
