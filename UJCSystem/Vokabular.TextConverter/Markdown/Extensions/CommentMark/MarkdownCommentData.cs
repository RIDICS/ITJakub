namespace Vokabular.TextConverter.Markdown.Extensions.CommentMark
{
    public class MarkdownCommentData
    {
        public string Identifier { get; set; }
        public string StartTag { get; set; }
        public string EndTag { get; set; }
        public bool IsIdValid { get; set; }
        public bool ContainsBothTags => !string.IsNullOrEmpty(StartTag) && !string.IsNullOrEmpty(EndTag);
    }
}