using System.IO;

namespace Vokabular.RestClient.Results
{
    public class FileResultData
    {
        public Stream Stream { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public long? FileSize { get; set; }
    }
}
