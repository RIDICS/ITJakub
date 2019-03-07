using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Parsers
{
    public interface IParser
    {
        ProjectImportMetadata Parse(ProjectImportMetadata projectImportMetadata, Dictionary<ParserHelperTypes, string> config);

        string BibliographicFormatName { get; }
    }
}
