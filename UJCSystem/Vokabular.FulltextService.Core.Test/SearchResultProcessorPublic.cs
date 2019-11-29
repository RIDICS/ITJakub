using System.Collections.Generic;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.TextConverter.Markdown;

namespace Vokabular.FulltextService.Core.Test
{
    public class SearchResultProcessorPublic : SearchResultProcessor
    {
        public SearchResultProcessorPublic(IMarkdownToPlainTextConverter markdownToPlainTextConverter) : base(markdownToPlainTextConverter)
        {
        }

        public int GetNumberOfHighlitOccurencesPublic(string highlightedText, string highlightTag)
        {
            return GetNumberOfHighlitOccurences(highlightedText, highlightTag);
        }

        public List<CorpusSearchResultContract> GetCorpusSearchResultDataListPublic(string highlightedText, long projectId,
            string highlightTag)
        {
            return GetCorpusSearchResultDataList(highlightedText, projectId, highlightTag);
        }
    }
}
