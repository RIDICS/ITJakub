using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Parsers
{
    public interface IProjectParser
    {
        ProjectImportMetadata Parse(ProjectImportMetadata projectImportMetadata);

        IList<PairIdValue> GetListPairIdValue(ProjectImportMetadata projectImportMetadata);

        string BibliographicFormatName { get; }
    }
}
