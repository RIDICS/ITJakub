﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nest;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.TextConverter.Markdown;

namespace Vokabular.FulltextService.Core.Helpers
{
    public class SearchResultProcessor
    {
        private readonly IMarkdownToPlainTextConverter m_markdownToPlainTextConverter;

        public SearchResultProcessor(IMarkdownToPlainTextConverter markdownToPlainTextConverter)
        {
            m_markdownToPlainTextConverter = markdownToPlainTextConverter;
        }

        public FulltextSearchResultContract ProcessSearchByCriteriaCount(ICountResponse response)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
            }

            return new FulltextSearchResultContract { Count = response.Count };
        }        

        public FulltextSearchResultContract ProcessSearchByCriteria(ISearchResponse<SnapshotResourceContract> response)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
            }

            return new FulltextSearchResultContract{ ProjectIds = response.Documents.Select(d => d.ProjectId).ToList() };
        }
        
        public FulltextSearchCorpusResultContract ProcessSearchCorpusByCriteriaCount(ISearchResponse<SnapshotResourceContract> response, string highlightTag)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
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

        protected int GetNumberOfHighlitOccurences(string highlightedText, string highlightTag)
        {
            return highlightedText.Split(new[] { highlightTag }, StringSplitOptions.None).Length / 2;
        }
        
        public TextResourceContract ProcessSearchPageByCriteria(ISearchResponse<TextResourceContract> response)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
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
                throw new FulltextDatabaseException(response.DebugInformation);
            }

            var result = new PageSearchResultContract
            {
                TextIdList = response.Hits.Select(hit => hit.Id).ToList()
            };

            return result;
        }

        public long ProcessSearchPageResultCount(ISearchResponse<TextResourceContract> response, string highlightTag)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
            }

            long counter = 0;
            foreach (var hit in response.Hits)
            {
                foreach (var value in hit.Highlights.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        counter += GetNumberOfHighlitOccurences(highlight, highlightTag);
                    }
                }
            }

            return counter;
        }

        protected void AddPageIdsToResult(List<CorpusSearchResultContract> resultData, List<SnapshotPageResourceContract> sourcePages)
        {
            foreach (var searchResultData in resultData)
            {
                var snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.After, @" <([^>]*)> ").Groups[1].Value;
                if (string.IsNullOrWhiteSpace(snapshotIndex))
                {
                    snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.Before, @" <([^>]*)> ").Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(snapshotIndex))
                    {
                        snapshotIndex = Regex.Match(searchResultData.PageResultContext.ContextStructure.Match, @" <([^>]*)> ").Groups[1].Value;
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
            contextStructure.After = Regex.Replace(contextStructure.After, @"^<[0-9]*>[ ]|[ ]<[0-9]*>[ ]|[ ]<[0-9]*>$|[ ]<[0-9]*$", "");
            //contextStructure.After = Regex.Replace(contextStructure.After, @"[ ]<[0-9]*>[ ]", "");
            //contextStructure.After = Regex.Replace(contextStructure.After, @"[ ]<[0-9]*>$", "");
            //contextStructure.After = Regex.Replace(contextStructure.After, @"[ ]<[0-9]*$", "");

            contextStructure.Before = Regex.Replace(contextStructure.Before, @"^[0-9]*>[ ]?|^<[0-9]*>[ ]|[ ]<[0-9]*>[ ]|[ ]<[0-9]*>$", "");
            //contextStructure.Before = Regex.Replace(contextStructure.Before, @"^<[0-9]*>[ ]", "");
            //contextStructure.Before = Regex.Replace(contextStructure.Before, @"[ ]<[0-9]*>[ ]", "");
            //contextStructure.Before = Regex.Replace(contextStructure.Before, @"[ ]<[0-9]*>$", "");

            contextStructure.Match = Regex.Replace(contextStructure.Match, @"^[0-9]*>[ ]|^<[0-9]*>[ ]|[ ]<[0-9]*>[ ]|[ ]<[0-9]*>$|[ ]<[0-9]*$", "");
            //contextStructure.Match = Regex.Replace(contextStructure.Match, @"^<[0-9]*>[ ]", "");
            //contextStructure.Match = Regex.Replace(contextStructure.Match, @"[ ]<[0-9]*>[ ]", "");
            //contextStructure.Match = Regex.Replace(contextStructure.Match, @"[ ]<[0-9]*>$", "");
            //contextStructure.Match = Regex.Replace(contextStructure.Match, @"[ ]<[0-9]*$", "");
        }

        protected List<CorpusSearchResultContract> GetCorpusSearchResultDataList(string highlightedText, long projectId, string highlightTag)
        {
            var result = new List<CorpusSearchResultContract>();

            var index = highlightedText.IndexOf(highlightTag, StringComparison.Ordinal);

            do
            {
                var contextStructure = GetContextStructure(highlightedText, index, out index, highlightTag);

                if (contextStructure != null)
                {
                    var corpusSearchResult = new CorpusSearchResultContract
                    {
                        ProjectId = projectId,
                        PageResultContext = new CorpusSearchPageResultContract { ContextStructure = contextStructure },
                    };

                    result.Add(corpusSearchResult);
                    break;
                }
                
                index = highlightedText.IndexOf(highlightTag, index + highlightTag.Length, StringComparison.Ordinal);
            } while (index > 0);

            return result;
        }

        private KwicStructure GetContextStructure(string highlightedText, int index, out int newIndex, string highlightTag)
        {
            newIndex = highlightedText.IndexOf(highlightTag, index + 1, StringComparison.Ordinal);
            if (newIndex < 0)
            {
                return null;
            }

            var before = highlightedText.Substring(0, index);
            var match = highlightedText.Substring(index + highlightTag.Length, newIndex - (index + highlightTag.Length));
            var after = highlightedText.Substring(newIndex + highlightTag.Length, highlightedText.Length - (newIndex + highlightTag.Length));

            before = before.Replace(highlightTag, "");
            after = after.Replace(highlightTag, "");

            return new KwicStructure {After = after, Before = before, Match = match};
        }

        public CorpusSearchSnapshotsResultContract ProcessSearchCorpusSnapshotsByCriteria(ISearchResponse<SnapshotResourceContract> response)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
            }
            
            return new CorpusSearchSnapshotsResultContract
            {
                TotalCount = response.Total,
                SnapshotList = response.Documents.Select(x => new CorpusSearchSnapshotContract{ SnapshotId = x.SnapshotId }).ToList()
            };
        }
        
        public List<CorpusSearchResultContract> ProcessSearchCorpusSnapshotByCriteria( ISearchResponse<SnapshotResourceContract> response, string highlightTag, int start, int count)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
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
                throw new FulltextDatabaseException(response.DebugInformation);
            }

            List<CorpusSearchSnapshotContract> snapshotWithCountList = new List<CorpusSearchSnapshotContract>();

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

                snapshotWithCountList.Add(new CorpusSearchSnapshotContract{ SnapshotId = hit.Source.SnapshotId, ResultCount = resultsCounter });
            }

            return new CorpusSearchSnapshotsResultContract
            {
                SnapshotList = snapshotWithCountList,
                TotalCount = response.Total,
            };
        }


        public HitsWithPageContextResultContract ProcessSearchHitsWithPageContext(ISearchResponse<TextResourceContract> response, List<SnapshotPageResourceContract> pageList, string highlightTag, int start, int count)
        {
            if (!response.IsValid)
            {
                throw new FulltextDatabaseException(response.DebugInformation);
            }

            var sortedPageList = pageList.OrderBy(x => x.PageIndex);

            int startCounter = 0;

            var resultList = new List<PageResultContextData>();


            foreach (var page in sortedPageList)
            {
                foreach (var hit in response.Hits.Where(x => x.Id == page.Id))
                {
                    foreach (var value in hit.Highlights.Values)
                    {
                        foreach (var highlight in value.Highlights)
                        {
                            var highlightPlain = m_markdownToPlainTextConverter.Convert(highlight);

                            var numberOfOccurences = GetNumberOfHighlitOccurences(highlightPlain, highlightTag);

                            if (startCounter + numberOfOccurences <= start)
                            {
                                startCounter += numberOfOccurences;
                                continue;
                            }

                            var resultData = GetSearchHitsWithPageContextList(highlightPlain, hit.Id, highlightTag);

                            if (startCounter < start)
                            {
                                resultData.RemoveRange(0, start - startCounter);
                                startCounter += resultData.Count;
                            }

                            if (resultList.Count + resultData.Count > count)
                                resultData = resultData.GetRange(0, count - resultList.Count);

                            resultList.AddRange(resultData);

                            if (resultList.Count == count)
                                return new HitsWithPageContextResultContract { ResultList = resultList };
                        }
                    }
                }
            }
            return new HitsWithPageContextResultContract { ResultList = resultList };
        }

        private List<PageResultContextData> GetSearchHitsWithPageContextList(string highlightedText, string sourceId, string highlightTag)
        {
            var result = new List<PageResultContextData>();

            var index = highlightedText.IndexOf(highlightTag, StringComparison.Ordinal);

            do
            {
                var contextStructure = GetContextStructure(highlightedText, index, out index, highlightTag);

                if (contextStructure != null)
                {
                    var corpusSearchResult = new PageResultContextData
                    {
                        PageExternalId = sourceId,
                        ContextStructure = contextStructure,
                    };

                    result.Add(corpusSearchResult);
                    break;
                }
                
                index = highlightedText.IndexOf(highlightTag, index + highlightTag.Length, StringComparison.Ordinal);
            } while (index > 0);

            return result;
        }
    }
}