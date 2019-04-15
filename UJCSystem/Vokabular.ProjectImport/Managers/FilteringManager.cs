using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.ProjectImport.Managers
{
    public class FilteringManager
    {
        private readonly ImportedProjectMetadataManager m_importedProjectMetadataManager;
        private readonly ImportHistoryManager m_importHistoryManager;

        public FilteringManager(ImportedProjectMetadataManager importedProjectMetadataManager, ImportHistoryManager importHistoryManager)
        {
            m_importedProjectMetadataManager = importedProjectMetadataManager;
            m_importHistoryManager = importHistoryManager;
        }

        public ImportedRecord SetFilterData(ImportedRecord importedRecord, IDictionary<string, List<string>> filteringExpressions,
            IProjectParser parser)
        {
            if (importedRecord.IsDeleted.HasValue && importedRecord.IsDeleted.Value)
            {
                importedRecord.IsSuitable = false;
                return importedRecord;
            }

            var importedRecordDb = m_importedProjectMetadataManager.GetImportedProjectMetadataByExternalId(importedRecord.ExternalId);
            importedRecord.IsNew = importedRecordDb?.Project == null;

            if (importedRecord.IsNew)
            {
                foreach (var item in parser.GetPairKeyValueList(importedRecord))
                {
                    filteringExpressions.TryGetValue(item.Key, out var filterExpressions);
                    if (filterExpressions == null)
                    {
                        continue;
                    }

                    if (filterExpressions.Select(Regex.Escape).Select(expr => expr.Replace("%", ".*"))
                        .Any(expr => Regex.IsMatch(item.Value, expr)))
                    {
                        importedRecord.IsSuitable = true;
                        return importedRecord;
                    }
                }

                importedRecord.IsSuitable = false;
            }
            else
            {
                var importHistory = m_importHistoryManager.GetLastImportHistoryForImportedProjectMetadata(importedRecordDb.Id);

                if (importedRecord.TimeStamp.HasValue && importHistory != null && importHistory.Date >= importedRecord.TimeStamp.Value)
                {
                    importedRecord.IsSuitable = false;
                }
                else
                {
                    importedRecord.ProjectId = importedRecordDb.Project.Id;
                    importedRecord.ImportedProjectMetadataId = importedRecordDb.Id;
                    importedRecord.IsSuitable = true;
                }
            }

            return importedRecord;
        }
    }
}