using System.Collections.Generic;
using Vokabular.ProjectImport.DataEntities.Database;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Parsers
{
    public interface IParser
    {
        Project Parse(string input, Dictionary<ParserHelperTypes, string> config);

        ParserType ParserType { get; }
    }
}
