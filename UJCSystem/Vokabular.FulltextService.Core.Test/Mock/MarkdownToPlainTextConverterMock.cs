using Vokabular.TextConverter.Markdown;

namespace Vokabular.FulltextService.Core.Test.Mock
{
    public class MarkdownToPlainTextConverterMock : IMarkdownToPlainTextConverter
    {
        public string Convert(string text)
        {
            return text;
        }
    }
}
