using Markdig;
using Vokabular.TextConverter.Markdown.Extensions.CommentMark;

namespace Vokabular.TextConverter.Markdown
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
                .UseCommentMarks()
                .Build();

            var result = Markdig.Markdown.ToHtml(markdownText, pipeline);
            return result;
        }
    }
}