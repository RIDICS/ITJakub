using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Model.Parsers
{
    public interface IProjectParser
    {
        /// <summary>
        /// The method parses the ImportedRecord.RawData and saves them to the ImportedRecord.ImportedProject.
        /// </summary>
        /// <param name="importedRecord"></param>
        /// <returns></returns>
        ImportedRecord AddParsedProject(ImportedRecord importedRecord);

        /// <summary>
        /// The method creates a list of keys and values from the ImportedRecord.RawData.
        /// </summary>
        /// <param name="importedRecord"></param>
        /// <returns></returns>
        IList<KeyValuePair<string, string>> GetPairKeyValueList(ImportedRecord importedRecord);

        string BibliographicFormatName { get; }
    }
}
