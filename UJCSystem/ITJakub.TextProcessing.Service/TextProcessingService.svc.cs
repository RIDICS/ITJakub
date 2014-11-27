using System.ServiceModel;
using ITJakub.TextProcessing.DataContracts;

namespace ITJakub.TextProcessing.Service
{
    public class TextProcessingService : ITextProcessingServiceLocal
    {
    
    }

    [ServiceContract]
    public interface ITextProcessingServiceLocal : ITextProcessingService
    {
    }
}