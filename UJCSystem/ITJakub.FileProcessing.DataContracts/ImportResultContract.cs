using System.Runtime.Serialization;

namespace ITJakub.FileProcessing.DataContracts
{
    [DataContract]
    public class ImportResultContract
    {
        [DataMember]
        public long ProjectId { get; set; }

        [DataMember]
        public bool Success { get; set; }

        public ImportResultContract()
        { }

        public ImportResultContract(long projectId, bool success)
        {
            ProjectId = projectId;
            Success = success;
        }
     }
}
