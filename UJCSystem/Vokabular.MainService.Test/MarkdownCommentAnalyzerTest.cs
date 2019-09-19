using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.TextConverter.Markdown.Extensions.CommentMark;

namespace Vokabular.MainService.Test
{
    [TestClass]
    public class MarkdownCommentAnalyzerTest
    {
        [TestMethod]
        public void TestAnalyzerAllCorrect()
        {
            var testText =
                "Lorem ipsum dolor sit amet, $komentar-5%consectetuer adipiscing elit%komentar-5$. Nullam sit amet magna in magna gravida vehicula. $komentar-3%Aliquam%komentar-3$ in lorem sit amet leo accumsan lacinia.";
            var analyzer = new MarkdownCommentAnalyzer();
            var result = analyzer.FindAllComments(testText);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0].IsValid);
            Assert.IsNotNull(result[0].StartTag);
            Assert.IsNotNull(result[0].EndTag);

            Assert.IsTrue(result[1].IsValid);
            Assert.IsNotNull(result[1].StartTag);
            Assert.IsNotNull(result[1].EndTag);
        }

        [TestMethod]
        public void TestAnalyzerBrokenComments()
        {
            var testText =
                "Lorem ipsum dolor sit amet, $komentar-5%consectetuer adipiscing elit. Nullam sit amet magna in magna gravida vehicula. $komentar-3%Aliquam%komentar-5$ in lorem sit amet leo accumsan lacinia.";
            var analyzer = new MarkdownCommentAnalyzer();
            var result = analyzer.FindAllComments(testText);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0].IsValid);
            Assert.IsNotNull(result[0].StartTag);
            Assert.IsNotNull(result[0].EndTag);

            Assert.IsTrue(result[1].IsValid);
            Assert.IsNotNull(result[1].StartTag);
            Assert.IsNull(result[1].EndTag);
        }

        [TestMethod]
        public void TestAnalyzerAcceptInvalid()
        {
            var testText =
                "Lorem ipsum dolor sit amet, $random-49-string%consectetuer adipiscing elit%differentstring$. Nullam sit amet magna in magna gravida vehicula.";
            var analyzer = new MarkdownCommentAnalyzer();
            var result = analyzer.FindAllComments(testText);

            Assert.AreEqual(2, result.Count);
            Assert.IsFalse(result[0].IsValid);
            Assert.IsNotNull(result[0].StartTag);
            Assert.IsNull(result[0].EndTag);

            Assert.IsFalse(result[1].IsValid);
            Assert.IsNull(result[1].StartTag);
            Assert.IsNotNull(result[1].EndTag);
        }
    }
}