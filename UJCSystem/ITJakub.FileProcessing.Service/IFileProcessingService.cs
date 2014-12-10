using System.IO;
using System.ServiceModel;

namespace ITJakub.FileProcessing.Service
{
    [ServiceContract]
    public interface IFileProcessingService
    {
        [OperationContract]
        void AddResource(string sessionId,string fileName, Stream dataStream);

        [OperationContract]
        bool ProcessSession(string sessionId);
    }
}