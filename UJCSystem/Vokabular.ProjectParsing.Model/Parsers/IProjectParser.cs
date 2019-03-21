using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Model.Parsers
{
    public interface IProjectParser
    {
        ProjectImportMetadata Parse(ProjectImportMetadata projectImportMetadata);

        IList<KeyValuePair<string, string>> GetListPairIdValue(ProjectImportMetadata projectImportMetadata);

        string BibliographicFormatName { get; }
    }
}
