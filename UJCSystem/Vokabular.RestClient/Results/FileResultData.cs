using System.IO;

namespace Vokabular.RestClient.Results
{
    public class FileResultData
    {
        public Stream Stream { get; set; }
        public string MimeType { get; set; }
    }
}
