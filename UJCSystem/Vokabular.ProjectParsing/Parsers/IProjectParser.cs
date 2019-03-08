using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Parsers
{
    public interface IProjectParser
    {
        ProjectImportMetadata Parse(ProjectImportMetadata projectImportMetadata, Dictionary<ParserHelperTypes, string> config);

        string BibliographicFormatName { get; }
    }
}
