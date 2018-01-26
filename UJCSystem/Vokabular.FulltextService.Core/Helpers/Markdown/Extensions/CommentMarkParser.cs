using Markdig.Helpers;
using Markdig.Parsers;
using Microsoft.Extensions.Options;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentMarkParser : InlineParser
    {
        private readonly string m_escapeChar;

        private static readonly char[] m_openingCharacters =
        {
            '$'
        };

        public CommentMarkParser(IOptions<Options.SpecialCharsOption> options)
        {
            OpeningCharacters = m_openingCharacters;
            m_escapeChar = options.Value.EscapeCharacter;
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            bool matchFound = false;
            char previous;
            
            previous = slice.PeekCharExtra(-1);

            if (previous.IsWhiteSpaceOrZero() || previous == '(' || previous == '[')
            {
                char current;
                int start;
                int endOfId;
                int endOfComment;
                int end;

                slice.NextChar();

                current = slice.CurrentChar;
                start = slice.Start;
                endOfComment = start;
                end = start;

                while (current != '%')
                {
                    current = slice.NextChar();
                }
                endOfId = slice.Start;
                current = slice.NextChar();
                

                while (current != '%')
                {
                    endOfComment = slice.Start;
                    current = slice.NextChar();
                }

                while (current != '$')
                {
                    end = slice.Start;
                    current = slice.NextChar();
                }

                if (current == '$')
                {
                    int inlineStart;
                    current = slice.NextChar();
                    inlineStart = processor.GetSourcePosition(slice.Start, out int line, out int column);

                    processor.Inline = new CommentMark()
                    {
                        Span =
                        {
                            Start = inlineStart,
                            End = inlineStart + (end - start) + 1
                        },
                        Line = line,
                        Column = column,
                        CommentId = new StringSlice(slice.Text, start, endOfId - 1),
                        CommentContext = new StringSlice(slice.Text, endOfId + 1, endOfComment)
                    };

                    matchFound = true;
                }
            }

            return matchFound;
        }
    }
}