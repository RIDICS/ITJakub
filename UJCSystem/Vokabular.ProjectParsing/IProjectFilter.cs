using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing
{
    public interface IProjectFilter
    {
        ProjectImportMetadata Filter(ProjectImportMetadata projectImport, IDictionary<string, List<string>> filteringExpressions);

        string BibliographicFormatName { get; }
    }
}
