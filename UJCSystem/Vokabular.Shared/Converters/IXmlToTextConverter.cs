using System.IO;

namespace Vokabular.Shared.Converters
{
    public interface IXmlToTextConverter
    {
        string Convert(Stream stream);
    }
}