using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing
{
    public interface IProjectFilter
    {
        ProjectImportMetadata Filter(ProjectImportMetadata projectImport);

        string BibliographicFormatName { get; }
    }
}
