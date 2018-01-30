using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nest;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search.Corpus;

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

        //TODO Obsolete method
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

        //TODO Obsolete method
        public List<CorpusSearchResultContract> ProcessSearchCorpusByCriteria(ISearchResponse<SnapshotResourceContract> response, string highlightTag, int start, int count)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            var startCounter = 0;

            var resultList = new List<CorpusSearchResultContract>();
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

                        if (resultList.Count + resultData.Count > count)
                            resultData = resultData.GetRange(0, count - resultList.Count);

                        resultList.AddRange(resultData);

                        if (resultList.Count == count)
                            return resultList;
                    }
                }
            }
            return resultList;
        }

        public List<CorpusSearchResultContract> ProcessSearchCorpusByCriteria(ISearchResponse<SnapshotResourceContract> response, string highlightTag)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
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

        public TextResourceContract ProcessSearchPageByCriteria(ISearchResponse<TextResourceContract> response)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
            foreach (var hit in response.Hits)
            {
                foreach (var value in hit.Highlights.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        hit.Source.PageText = highlight;
                        return hit.Source;
                    }
                }
            }
            return null;
        }

        public PageSearchResultContract ProcessSearchPageResult(ISearchResponse<TextResourceContract> response)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            var result = new PageSearchResultContract
            {
                TextIdList = response.Hits.Select(hit => hit.Id).ToList()
            };

            return result;
        }

        private void AddPageIdsToResult(List<CorpusSearchResultContract> resultData, List<SnapshotPageResourceContract> sourcePages)
        {
            foreach (var searchResultData in resultData)
            {
                var snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.After, @"[^\\]<([^>]*)>").Groups[1].Value;
                if (string.IsNullOrWhiteSpace(snapshotIndex))
                {
                    snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.Before, @"[^\\]<([^>]*)>").Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(snapshotIndex))
                    {
                        snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.Match, @"[^\\]<([^>]*)>").Groups[1].Value;
                    }
                }
                var pageId = GetPageIdFromIndex(Int32.Parse(snapshotIndex), sourcePages);
                searchResultData.PageResultContext.TextExternalId = pageId;

                RemovePageIds(searchResultData.PageResultContext.ContextStructure);
            }
        }

        private string GetPageIdFromIndex(int pageIndex, List<SnapshotPageResourceContract> sourcePages)
        {
            return sourcePages.Where(page => page.PageIndex == pageIndex).Select(page => page.Id).FirstOrDefault();
        }

        private void RemovePageIds(KwicStructure contextStructure)
        {
            contextStructure.After = Regex.Replace(contextStructure.After, @"^<[0-9]*>[ ]", "");
            contextStructure.After = Regex.Replace(contextStructure.After, @"[ ]<[0-9]*>[ ]", "");
            contextStructure.After = Regex.Replace(contextStructure.After, @"[ ]<[0-9]*>$", "");
            contextStructure.After = Regex.Replace(contextStructure.After, @"[ ]<[0-9]*$", "");

            contextStructure.Before = Regex.Replace(contextStructure.Before, @"^[0-9]*>[ ]?", "");
            contextStructure.Before = Regex.Replace(contextStructure.Before, @"^<[0-9]*>[ ]", "");
            contextStructure.Before = Regex.Replace(contextStructure.Before, @"[ ]<[0-9]*>[ ]", "");
            contextStructure.Before = Regex.Replace(contextStructure.Before, @"[ ]<[0-9]*>$", "");

            contextStructure.Match = Regex.Replace(contextStructure.Match, @"^[0-9]*>[ ]", "");
            contextStructure.Match = Regex.Replace(contextStructure.Match, @"^<[0-9]*>[ ]", "");
            contextStructure.Match = Regex.Replace(contextStructure.Match, @"[ ]<[0-9]*>[ ]", "");
            contextStructure.Match = Regex.Replace(contextStructure.Match, @"[ ]<[0-9]*>$", "");
            contextStructure.Match = Regex.Replace(contextStructure.Match, @"[ ]<[0-9]*$", "");
        }

        private List<CorpusSearchResultContract> GetCorpusSearchResultDataList(string highlightedText, long projectId, string highlightTag)
        {
            var result = new List<CorpusSearchResultContract>();

            var index = highlightedText.IndexOf(highlightTag, StringComparison.Ordinal);

            do
            {
                var corpusSearchResult = GetCorpusSearchResultData(highlightedText, projectId, index, out index, highlightTag);
                result.Add(corpusSearchResult);

                index = highlightedText.IndexOf(highlightTag, index + highlightTag.Length, StringComparison.Ordinal);
            } while (index > 0);

            return result;
        }

        private CorpusSearchResultContract GetCorpusSearchResultData(string highlightedText, long projectId, int index, out int newIndex, string highlightTag)
        {
            newIndex = highlightedText.IndexOf(highlightTag, index + 1, StringComparison.Ordinal);

            var before = highlightedText.Substring(0, index);
            var match = highlightedText.Substring(index + highlightTag.Length, newIndex - (index + highlightTag.Length));
            var after = highlightedText.Substring(newIndex + highlightTag.Length, highlightedText.Length - (newIndex + highlightTag.Length));

            before = before.Replace(highlightTag, "");
            after = after.Replace(highlightTag, "");

            return new CorpusSearchResultContract
            {
                ProjectId = projectId,
                PageResultContext = new CorpusSearchPageResultContract
                {
                    ContextStructure = new KwicStructure { After = after, Before = before, Match = match },
                }
            };
        }

        public CorpusSearchSnapshotsResultContract ProcessSearchCorpusSnapshotsByCriteria(ISearchResponse<SnapshotResourceContract> response)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
            
            return new CorpusSearchSnapshotsResultContract
            {
                TotalCount = response.Total,
                SnapshotIds = response.Documents.Select(x => x.SnapshotId).ToList()
            };
        }


        public List<CorpusSearchResultContract> ProccessSearchCorpusSnapshotByCriteria( ISearchResponse<SnapshotResourceContract> response, string highlightTag, int start, int count)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            int startCounter = 0;
            
            var resultList = new List<CorpusSearchResultContract>();
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

                        if (resultList.Count + resultData.Count > count)
                            resultData = resultData.GetRange(0, count - resultList.Count);

                        resultList.AddRange(resultData);

                        if (resultList.Count == count)
                            return resultList;
                    }
                }
            }
            
            return resultList;
        }

        public CorpusSearchSnapshotsResultContract ProcessSearchCorpusSnapshotsByCriteriaFetchResultCount(ISearchResponse<SnapshotResourceContract> response, string highlightTag)
        {
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            Dictionary<long, long> resultsCount = new Dictionary<long, long>();

            foreach (var hit in response.Hits)
            {
                long resultsCounter = 0;
                foreach (var value in hit.Highlights.Values)
                {
                    
                    foreach (var highlight in value.Highlights)
                    {
                        var numberOfOccurences = GetNumberOfHighlitOccurences(highlight, highlightTag);
                        resultsCounter += numberOfOccurences;
                    }
                }
                resultsCount.Add(hit.Source.SnapshotId, resultsCounter);
            }

            return new CorpusSearchSnapshotsResultContract
            {
                ResultsInSnapshotsCount = resultsCount,
                TotalCount = response.Total,
            };
        }
    }
}