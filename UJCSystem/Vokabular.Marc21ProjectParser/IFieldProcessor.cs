using System.Collections.Generic;

namespace Vokabular.Marc21ProjectParser
{
    public interface IFieldProcessor
    {
        IList<string> Tags { get; }
    }
}