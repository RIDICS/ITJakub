namespace Vokabular.MainService.DataContracts.Contracts
{
    public class RepositoryImportProgressInfoContract
    {
        public int TotalProjectsCount { get; set; }
        public int ExternalRepositoryId { get; set; }
        public bool IsCompleted { get; set; }
        public string FaultedMessage { get; set; }
        public int ProcessedProjectsCount { get; set; }
        public int FailedProjectsCount { get; set; }
    }
}
