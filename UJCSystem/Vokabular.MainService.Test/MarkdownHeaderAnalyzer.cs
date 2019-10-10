using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.TextConverter.Markdown.Extensions;

namespace Vokabular.MainService.Test
{
    [TestClass]
    public class MarkdownHeaderAnalyzerTest
    {
        [TestMethod]
        public void TestHeaderAnalyzer()
        {
            var testText = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
### Etiam quis quam.
#Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos.
  ## # Maecenas sollicitudin.
Mauris ## suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus.
	#	Aliquam erat volutpat.
Nulla non lectus sed nisl molestie malesuada.";

            var analyzer = new MarkdownHeaderAnalyzer();
            var result = analyzer.FindAllHeaders(testText);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(3, result[0].Level);
            Assert.AreEqual("Etiam quis quam.", result[0].Header);

            Assert.AreEqual(2, result[1].Level);
            Assert.AreEqual("# Maecenas sollicitudin.", result[1].Header);

            Assert.AreEqual(1, result[2].Level);
            Assert.AreEqual("Aliquam erat volutpat.", result[2].Header);
        }
    }
}