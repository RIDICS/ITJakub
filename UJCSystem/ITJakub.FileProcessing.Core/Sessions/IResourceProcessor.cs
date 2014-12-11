using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public interface IResourceProcessor
    {
        IEnumerable<string> SupportedFormats { get; }
    }

    public class ImageProcessor : IResourceProcessor
    {
        public IEnumerable<string> SupportedFormats { get; private set; }
    }

    public class DocxProcessor : IResourceProcessor
    {
        public IEnumerable<string> SupportedFormats { get; private set; }
    }
}