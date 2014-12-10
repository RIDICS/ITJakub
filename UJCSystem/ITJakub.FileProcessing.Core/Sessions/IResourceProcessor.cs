using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public interface IResourceProcessor
    {
        IEnumerable<string> SupportedFormats { get; }
    }

    public class ImageProcessor
    {
        
    }

    public class DocxProcessor
    {
        
    }
}