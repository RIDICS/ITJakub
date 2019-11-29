using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.FulltextService.Core.Test.Mock;

namespace Vokabular.FulltextService.Core.Test
{
    [TestClass]
    public class SearchResultProcessorTest
    {
        private readonly SearchResultProcessorPublic m_searchResultProcessor;

        public SearchResultProcessorTest()
        {
            var markdownToPlainTextConverter = new MarkdownToPlainTextConverterMock();
            m_searchResultProcessor = new SearchResultProcessorPublic(markdownToPlainTextConverter);
        }

        [TestMethod]
        public void TestCountOfHighlightOccurences()
        {
            const string tag = "!";
            const string highlightedText = "Lorem ipsum dolor sit amet, consectetuer adipiscing !elit!. Etiam sapien !elit!, consequat eget, tristique non! Venenatis";

            var count = m_searchResultProcessor.GetNumberOfHighlitOccurencesPublic(highlightedText, tag);
            
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void TestHighlightedTextSimple()
        {
            const string tag = "$";
            const string highlightedText = "Lorem ipsum $dolor$ sit amet, consectetuer adipiscing elit. Sed convallis magna eu sem. Duis viverra diam";

            var result = m_searchResultProcessor.GetCorpusSearchResultDataListPublic(highlightedText, 1, tag);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestHighlightedTextSplit()
        {
            const string tag = "¼";
            const string highlightedText =
                "<6> ero. Sadipscing <6>  ¼ipsum¼ at ¼ipsum <6> ¼ feugait et ¼ips <6> um¼ ¼ipsum¼ et est <6>  est sed sit al";

            var result = m_searchResultProcessor.GetCorpusSearchResultDataListPublic(highlightedText, 1, tag);

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual("<6> ero. Sadipscing <6>  ¼ipsum¼ at ¼ipsum <6> ¼ feugait et ", result[2].PageResultContext.ContextStructure.Before);
            Assert.AreEqual("ips <6> um", result[2].PageResultContext.ContextStructure.Match);
            Assert.AreEqual(" ¼ipsum¼ et est <6>  est sed sit al", result[2].PageResultContext.ContextStructure.After);
        }

        [TestMethod]
        public void TestHighlightedTextSplitWithHighlightCharInOriginalText()
        {
            const string tag = "!";
            const string highlightedText = "Lorem ipsum dolor sit amet, consectetuer adipiscing !elit!. Etiam sapien !elit!, consequat eget, tristique non! Venenatis";

            var result = m_searchResultProcessor.GetCorpusSearchResultDataListPublic(highlightedText, 1, tag);

            Assert.AreEqual(2, result.Count);
        }
    }
}
