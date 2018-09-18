using System.Runtime.Serialization;

namespace Vokabular.Shared
{
    [DataContract]
    public class ImportResult
    {
       
        [DataMember]
        public long ProjectId { get; set; }
        [DataMember]
        public long SnapshotId { get; set; }
        [DataMember]
        public bool Success { get; set; }

        public ImportResult()
        { }

        public ImportResult(long projectId, long snapshotId, bool success)
        {
            ProjectId = projectId;
            SnapshotId = snapshotId;
            Success = success;
        }
     }
}
