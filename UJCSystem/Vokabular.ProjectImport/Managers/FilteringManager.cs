using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.ProjectImport.Managers
{
    public class FilteringManager
    {
        private readonly ImportMetadataManager m_importMetadataManager;

        public FilteringManager(ImportMetadataManager importMetadataManager)
        {
            m_importMetadataManager = importMetadataManager;
        }

        public ProjectImportMetadata Filter(ProjectImportMetadata metadata, IDictionary<string, List<string>> filteringExpressions,
            IProjectParser parser)
        {
            var metadataDb = m_importMetadataManager.GetImportMetadataByExternalId(metadata.ExternalId);
            metadata.IsNew = metadataDb == null;

            if (metadata.IsNew)
            {
                foreach (var item in parser.GetListPairIdValue(metadata))
                {
                    filteringExpressions.TryGetValue(item.Key, out var filterExpressions);
                    if (filterExpressions == null)
                    {
                        continue;
                    }

                    if (filterExpressions.Select(Regex.Escape).Select(expr => expr.Replace("%", ".*"))
                        .Any(expr => Regex.IsMatch(item.Value, expr)))
                    {
                        metadata.IsSuitable = true;
                        return metadata;
                    }
                }

                metadata.IsSuitable = false;
            }
            else
            {
                metadata.ProjectId = metadataDb.Snapshot.Project.Id;
                metadata.IsSuitable = true;
            }

            return metadata;
        }
    }
}