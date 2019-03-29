using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Model.Parsers
{
    public interface IProjectParser
    {
        ImportedRecord AddParsedProject(ImportedRecord importedRecord);

        IList<KeyValuePair<string, string>> GetPairKeyValueList(ImportedRecord importedRecord);

        string BibliographicFormatName { get; }
    }
}
