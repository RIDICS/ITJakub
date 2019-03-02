﻿using System.Threading;

namespace Vokabular.ProjectImport.Model
{
    public class ProjectImportProgressInfo
    {
        public int TotalProjectsCount { get; set; }
        public int ExternalResourceId { get; }
        public bool IsCompleted { get; set; }
        public string FaultedMessage { get; set; }

        private int m_unusedProjectsCount;
        public int UnusedProjectsCount => m_unusedProjectsCount;
        public int IncrementUnusedProjectsCount() { return Interlocked.Increment(ref m_unusedProjectsCount); }

        private int m_processedProjectsCount;
        public int ProcessedProjectsCount => m_processedProjectsCount;
        public int IncrementProcessedProjectsCount() { return Interlocked.Increment(ref m_processedProjectsCount); }

        public ProjectImportProgressInfo(int externalResourceId)
        {
            ExternalResourceId = externalResourceId;
            m_unusedProjectsCount = 0;
            m_processedProjectsCount = 0;
            //TODO remove
            TotalProjectsCount = 5000;
        }
    }
}
