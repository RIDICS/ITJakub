using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.TextConverter.Markdown.Extensions;

namespace Vokabular.MainService.Test
{
    [TestClass]
    public class MarkdownHeadingAnalyzerTest
    {
        [TestMethod]
        public void TestHeadingAnalyzer()
        {
            var testText = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
### Etiam quis quam.
#Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos.
  ## # Maecenas sollicitudin.
Mauris ## suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus.
	#	Aliquam erat volutpat.
Nulla non lectus sed nisl molestie malesuada.";

            var analyzer = new MarkdownHeadingAnalyzer();
            var result = analyzer.FindAllHeadings(testText);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(3, result[0].Level);
            Assert.AreEqual("Etiam quis quam.", result[0].Heading);

            Assert.AreEqual(2, result[1].Level);
            Assert.AreEqual("# Maecenas sollicitudin.", result[1].Heading);

            Assert.AreEqual(1, result[2].Level);
            Assert.AreEqual("Aliquam erat volutpat.", result[2].Heading);
        }
    }
}