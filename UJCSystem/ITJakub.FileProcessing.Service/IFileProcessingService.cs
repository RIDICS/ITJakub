using System.ServiceModel;

namespace ITJakub.FileProcessing.Service
{
    [ServiceContract]
    public interface IFileProcessingService
    {
        [OperationContract]
        void TestXml();
    }
}