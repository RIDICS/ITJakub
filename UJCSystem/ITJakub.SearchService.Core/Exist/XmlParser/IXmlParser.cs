using System.IO;

namespace ITJakub.SearchService.Core.Exist.XmlParser
{
    public interface IXmlParser<T>
    {
        T Parse(Stream dataStream);
    }
}