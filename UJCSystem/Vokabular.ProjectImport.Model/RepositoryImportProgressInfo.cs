using System.Threading;

namespace Vokabular.ProjectImport.Model
{
    public class RepositoryImportProgressInfo
    {
        private int m_processedProjectsCount;
        private int m_failedProjectsCount;

        public RepositoryImportProgressInfo(int externalRepositoryId, string externalRepositoryName)
        {
            ExternalRepositoryId = externalRepositoryId;
            ExternalRepositoryName = externalRepositoryName;
            m_processedProjectsCount = 0;
            m_failedProjectsCount = 0;
        }

        public int TotalProjectsCount { get; set; }
        public int ExternalRepositoryId { get; }
        public string ExternalRepositoryName { get; }
        public bool IsCompleted { get; set; }
        public string FaultedMessage { get; set; }
        public object[] FaultedMessageParams { get; set; }
        public int ProcessedProjectsCount => m_processedProjectsCount;
        public int FailedProjectsCount => m_failedProjectsCount;

        public int IncrementProcessedProjectsCount() { return Interlocked.Increment(ref m_processedProjectsCount); }
        public int IncrementFailedProjectsCount() { return Interlocked.Increment(ref m_failedProjectsCount); }
    }
}
