using System.Collections.Generic;
using Project = Vokabular.ProjectParsing.Model.Entities.Project;

namespace Vokabular.ProjectParsing.Parsers
{
    public interface IParser
    {
        Project Parse(string input, Dictionary<ParserHelperTypes, string> config);

        string ParserTypeName { get; }
    }
}
