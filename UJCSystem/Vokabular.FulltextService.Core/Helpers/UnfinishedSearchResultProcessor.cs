using System.Collections.Generic;
using Nest;
using Vokabular.FulltextService.DataContracts.Contracts;

namespace Vokabular.FulltextService.Core.Helpers
{
    public class UnfinishedSearchResultProcessor : SearchResultProcessor
    {
        public List<CorpusSearchResultContract> ProcessSearchCorpusByCriteria(ISearchResponse<SnapshotResourceContract> response, string highlightTag)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
            }

            var resultList = new List<CorpusSearchResultContract>();
            foreach (var hit in response.Hits)
            {
                foreach (var value in hit.Highlights.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        var resultData = GetCorpusSearchResultDataList(highlight, hit.Source.ProjectId, highlightTag);
                        AddPageIdsToResult(resultData, hit.Source.Pages);

                        resultList.AddRange(resultData);
                    }
                }
            }
            resultList = resultList.Count > 100 ? resultList.GetRange(0, 100) : resultList; //TODO temporary returns only first 100 results
            return resultList;
        }
    }
}