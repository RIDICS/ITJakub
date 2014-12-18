using System.IO;
using System.ServiceModel;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.DataContracts
{
    [ServiceContract]
    public interface IFileProcessingService
    {
        [OperationContract]
        void AddResource(UploadResourceContract resourceInfoSkeleton);

        [OperationContract]
        bool ProcessSession(string sessionId, string uploadMessage);
    }
}