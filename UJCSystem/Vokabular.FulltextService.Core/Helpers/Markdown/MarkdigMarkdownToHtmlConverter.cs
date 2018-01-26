using Markdig;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Helpers.Markdown.Extensions;
using Vokabular.FulltextService.Core.Options;

namespace Vokabular.FulltextService.Core.Helpers.Markdown
{
    public class MarkdigMarkdownToHtmlConverter : IMarkdownToHtmlConverter
    {
        private readonly IOptions<SpecialCharsOption> m_options;

        public MarkdigMarkdownToHtmlConverter(IOptions<SpecialCharsOption> options)
        {
            m_options = options;
        }

        public string ConvertToHtml(string markdownText)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAbbreviations()
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmphasisExtras()
                .UseFigures()
                .UseFooters()
                .UseFootnotes()
                .UseGridTables()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                .UseCommentMarks(m_options)
                .Build();

            var result = Markdig.Markdown.ToHtml(markdownText, pipeline);
            return result;
        }
    }
}