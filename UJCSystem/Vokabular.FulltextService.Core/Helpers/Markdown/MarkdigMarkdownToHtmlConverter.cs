using Markdig;
using Vokabular.FulltextService.Core.Helpers.Markdown.Extensions;

namespace Vokabular.FulltextService.Core.Helpers.Markdown
{
    public class MarkdigMarkdownToHtmlConverter : IMarkdownToHtmlConverter
    {
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
                .Use<CommentExtension>()
                .Build();
            var result = Markdig.Markdown.ToHtml(markdownText, pipeline);
            return result;
        }
    }
}