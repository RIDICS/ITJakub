using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nest;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.Shared.DataContracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.ResultContracts;

namespace Vokabular.FulltextService.Core.Helpers
{
    public class SearchResultProcessor
    {
        public FulltextSearchResultContract ProcessSearchByCriteriaCount(ICountResponse response)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            return new FulltextSearchResultContract { Count = response.Count };
        }

        public FulltextSearchResultContract ProcessSearchByCriteria(ISearchResponse<SnapshotResourceContract> response)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            return new FulltextSearchResultContract{ ProjectIds = response.Documents.Select(d => d.ProjectId).ToList() };
        }

        public FulltextSearchCorpusResultContract ProcessSearchCorpusByCriteriaCount(ISearchResponse<SnapshotResourceContract> response, string highlightTag)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            var counter = 0;

            foreach (var highlightField in response.Hits.Select(d => d.Highlights))
            {
                foreach (var value in highlightField.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        counter += GetNumberOfHighlitOccurences(highlight, highlightTag);
                    }
                }
            }

            return new FulltextSearchCorpusResultContract { Count = counter };
        }

        private int GetNumberOfHighlitOccurences(string highlightedText, string highlightTag)
        {
            return highlightedText.Split(new[] { highlightTag }, StringSplitOptions.None).Length / 2;
        }

        public CorpusSearchResultDataList ProcessSearchCorpusByCriteria(ISearchResponse<SnapshotResourceContract> response, string highlightTag, int start, int count)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            var startCounter = 0;

            var result = new CorpusSearchResultDataList
            {
                List = new List<CorpusSearchResultData>(),
                SearchResultType = FulltextSearchResultType.ProjectId
            };
            foreach (var hit in response.Hits)
            {
                foreach (var value in hit.Highlights.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        var numberOfOccurences = GetNumberOfHighlitOccurences(highlight, highlightTag);

                        if (startCounter + numberOfOccurences <= start)
                        {
                            startCounter += numberOfOccurences;
                            continue;
                        }

                        var resultData = GetCorpusSearchResultDataList(highlight, hit.Source.ProjectId, highlightTag);
                        AddPageIdsToResult(resultData, hit.Source.Pages);

                        if (startCounter < start)
                        {
                            resultData.RemoveRange(0, start - startCounter);
                            startCounter += resultData.Count;
                        }

                        if (result.List.Count + resultData.Count > count)
                            resultData = resultData.GetRange(0, count - result.List.Count);

                        result.List.AddRange(resultData);

                        if (result.List.Count == count)
                            return result;
                    }
                }
            }
            return result;
        }

        public CorpusSearchResultDataList ProcessSearchCorpusByCriteria(ISearchResponse<SnapshotResourceContract> response, string highlightTag)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            var startCounter = 0;

            var result = new CorpusSearchResultDataList
            {
                List = new List<CorpusSearchResultData>(),
                SearchResultType = FulltextSearchResultType.ProjectId
            };
            foreach (var hit in response.Hits)
            {
                foreach (var value in hit.Highlights.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        var resultData = GetCorpusSearchResultDataList(highlight, hit.Source.ProjectId, highlightTag);
                        AddPageIdsToResult(resultData, hit.Source.Pages);

                        result.List.AddRange(resultData);
                    }
                }
            }
            result.List = result.List.Count > 100 ? result.List.GetRange(0, 100) : result.List; //TODO 
            return result;
        }

        private void AddPageIdsToResult(List<CorpusSearchResultData> resultData, List<SnapshotPageResourceContract> sourcePages)
        {
            foreach (var searchResultData in resultData)
            {
                var snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.After, @"<([^>]*)>").Groups[1].Value;
                if (string.IsNullOrWhiteSpace(snapshotIndex))
                {
                    snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.Before, @"<([^>]*)>").Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(snapshotIndex))
                    {
                        snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.Match, @"<([^>]*)>").Groups[1].Value;
                    }
                }
                var pageId = GetPageIdFromIndex(Int32.Parse(snapshotIndex), sourcePages);
                searchResultData.PageResultContext.TextExternalId = pageId;
                searchResultData.PageResultContext.ContextStructure.After = Regex.Replace(searchResultData.PageResultContext.ContextStructure.After, @"<[^>]*>", "");
                searchResultData.PageResultContext.ContextStructure.After = Regex.Replace(searchResultData.PageResultContext.ContextStructure.After, @"<[^>]*", "");
                searchResultData.PageResultContext.ContextStructure.Before = Regex.Replace(searchResultData.PageResultContext.ContextStructure.Before, @"<[^>]*>", "");
                searchResultData.PageResultContext.ContextStructure.Before = Regex.Replace(searchResultData.PageResultContext.ContextStructure.Before, @"[^>]*>", "");
                searchResultData.PageResultContext.ContextStructure.Match = Regex.Replace(searchResultData.PageResultContext.ContextStructure.Match, @"<[^>]*>", "");
                searchResultData.PageResultContext.ContextStructure.Match = Regex.Replace(searchResultData.PageResultContext.ContextStructure.Match, @"[^>]*>", "");
                searchResultData.PageResultContext.ContextStructure.Match = Regex.Replace(searchResultData.PageResultContext.ContextStructure.Match, @"<[^>]*", "");
            }
        }

        private string GetPageIdFromIndex(int snapshotIndex, List<SnapshotPageResourceContract> sourcePages)
        {
            return sourcePages.Where(page => page.indexFrom <= snapshotIndex && snapshotIndex <= page.indexTo).Select(page => page.Id).FirstOrDefault();
        }

        private List<CorpusSearchResultData> GetCorpusSearchResultDataList(string highlightedText, long projectId, string highlightTag)
        {
            var result = new List<CorpusSearchResultData>();

            var index = highlightedText.IndexOf(highlightTag, StringComparison.Ordinal);

            do
            {
                var corpusSearchResult = GetCorpusSearchResultData(highlightedText, projectId, index, out index, highlightTag);
                result.Add(corpusSearchResult);

                index = highlightedText.IndexOf(highlightTag, index + highlightTag.Length, StringComparison.Ordinal);
            } while (index > 0);

            return result;
        }

        private CorpusSearchResultData GetCorpusSearchResultData(string highlightedText, long projectId, int index, out int newIndex, string highlightTag)
        {
            newIndex = highlightedText.IndexOf(highlightTag, index + 1, StringComparison.Ordinal);

            var before = highlightedText.Substring(0, index);
            var match = highlightedText.Substring(index + highlightTag.Length, newIndex - (index + highlightTag.Length));
            var after = highlightedText.Substring(newIndex + highlightTag.Length, highlightedText.Length - (newIndex + highlightTag.Length));

            before = before.Replace(highlightTag, "");
            after = after.Replace(highlightTag, "");

            return new CorpusSearchResultData
            {
                ProjectId = projectId,
                PageResultContext = new CorpusSearchPageResultData
                {
                    ContextStructure = new KwicStructure { After = after, Before = before, Match = match },
                }
            };
        }


    }
}