using System.ServiceModel;
using ITJakub.FileProcessing.Service;

namespace ITJakub.XmlProcessingConsole
{
    public class ItJakubServiceClient : ClientBase<IFileProcessingService>, IFileProcessingService
    {
        public void TestXml()
        {
                Channel.TestXml();
        }
    }
}