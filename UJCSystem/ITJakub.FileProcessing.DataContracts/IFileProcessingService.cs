using System.IO;
using System.ServiceModel;

namespace ITJakub.FileProcessing.DataContracts
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