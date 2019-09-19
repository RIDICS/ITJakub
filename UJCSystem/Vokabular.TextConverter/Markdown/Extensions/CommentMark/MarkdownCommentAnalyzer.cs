using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vokabular.TextConverter.Markdown.Extensions.CommentMark
{
    public class MarkdownCommentAnalyzer
    {
        public IList<MarkdownCommentData> FindAllComments(string text)
        {
            var regexComment = new Regex("(\\$(\\w|\\d|-)*%|%(\\w|\\d|-)*\\$)");
            var regexIdentifier = new Regex($"{CommentMarkParser.CommentMark}\\d");
            var matches = regexComment.Matches(text);

            var resultList = new List<MarkdownCommentData>();
            var processedItems = new Dictionary<string, MarkdownCommentData>();

            foreach (Match match in matches)
            {
                var value = match.Value;
                var identifier = GetIdentifier(value);
                var isValid = regexIdentifier.IsMatch(identifier);

                if (value[0] == '$') //start tag
                {
                    var item = new MarkdownCommentData
                    {
                        StartTag = value,
                        Identifier = identifier,
                        IsValid = isValid,
                    };
                    processedItems.Add(item.Identifier, item);
                    resultList.Add(item);
                }
                else if (value[0] == '%') // end tag
                {
                    if (processedItems.TryGetValue(identifier, out var item))
                    {
                        item.EndTag = value;
                        processedItems.Remove(identifier);
                    }
                    else
                    {
                        item = new MarkdownCommentData
                        {
                            EndTag = value,
                            Identifier = identifier,
                            IsValid = isValid,
                        };
                        resultList.Add(item);
                    }
                }
            }

            return resultList;
        }

        private string GetIdentifier(string token)
        {
            return token.Substring(1, token.Length - 2);
        }
    }
}