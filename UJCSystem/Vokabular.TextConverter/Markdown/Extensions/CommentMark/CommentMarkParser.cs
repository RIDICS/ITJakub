using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Options;
using Vokabular.TextConverter.Options;

namespace Vokabular.TextConverter.Markdown.Extensions.CommentMark
{
    public class CommentMarkParser : InlineParser
    {
        private const string CommentMark = "komentar-";
        private readonly char m_closingChar;
        private readonly char m_escapeChar;
        private readonly char m_openingChar;

        public CommentMarkParser(IOptions<SpecialCharsOption> options)
        {
            m_escapeChar = options.Value.EscapeCharacter[0];
            m_openingChar = options.Value.CommentMarkOpening[0];
            m_closingChar = options.Value.CommentMarkClosing[0];

            OpeningCharacters = new[] { m_openingChar };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (IsCurrentCharEscaped(slice) || !IsBeginningOfTag(slice, m_closingChar)) //opening char is escaped or its not beginning of tag
            {
                return false;
            }

            processor.Inline = GetInlineContainer(processor, ref slice);
            return true;
        }
        
        private CommentMarkContainer GetInlineContainer(InlineProcessor processor, ref StringSlice slice)
        {
            var start = slice.Start;
            int newStart;
            slice.NextChar();

            var openingId = GetCommentId(ref slice);
            string closingId;
            
            var childList = new List<Inline>();

            do //While closing tag for this opening tag is not found
            {
                slice.Start = slice.Start + openingId.Length;
                
                newStart = slice.Start + 1;
                do //While closing char is escaped or it is not beginning of tag
                {
                    var currentChar = slice.NextChar();
                    while (currentChar != m_closingChar) //While closing char is not found
                    {
                        if (currentChar == m_openingChar && !IsCurrentCharEscaped(slice) && IsBeginningOfTag(slice, m_closingChar)) //Inner comment is found
                        {
                            var commentText = new CommentMarkText   //Text before inner comment
                            {
                                Text = new StringSlice(slice.Text, newStart, slice.Start - 1),
                            };
                            childList.Add(commentText);

                            var innerComment = GetInlineContainer(processor, ref slice); //Inner comment

                            if (innerComment != null)
                            {
                                childList.Add(innerComment);
                            }

                            newStart = slice.Start;
                            slice.Start = slice.Start - 1;
                        }
                        currentChar = slice.NextChar();

                        if (currentChar == '\0')//End of file
                        {
                            return null;
                            
                        } 
                    }
                } while (!IsBeginningOfTag(slice, m_openingChar) && IsCurrentCharEscaped(slice));

                slice.NextChar();
                closingId = slice.Text.Substring(slice.Start, openingId.Length);

            } while (!openingId.Equals(closingId));

            var lastCommentText = new CommentMarkText
            {
                Text = new StringSlice(slice.Text, newStart, slice.Start - 2),
            };
            childList.Add(lastCommentText);

            slice.Start = slice.Start + openingId.Length + 1;
            int inlineStart = processor.GetSourcePosition(start, out int line, out int column);

            return new CommentMarkContainer
            {
                Span =
                {
                    Start = inlineStart,
                    End = inlineStart + (slice.Start - start) + 2
                },
                Line = line,
                Column = column,
                CommentId = openingId,
                ChildList = childList,
            };
        }

        private bool IsCurrentCharEscaped(StringSlice slice)
        {
            var previous = slice.PeekCharExtra(-1);
            var counter = 0;

            while (previous == m_escapeChar)
            {
                counter++;
                previous = slice.PeekCharExtra(-1 * (counter + 1));
            }

            if (counter % 2 == 1)
            {
                return true;
            }

            return false;
        }

        private string GetCommentId(ref StringSlice slice)
        {
            var commentId = slice.Text.Substring(slice.Start, CommentMark.Length);
            var i = slice.Start + CommentMark.Length;
            while (slice.Text[i].IsDigit()) // add textId
            {
                commentId += slice.Text[i];
                i++;
            }
            
            return commentId;
        }

        private bool IsBeginningOfTag(StringSlice slice, char tagClosingChar)
        {
            var i = slice.Start + CommentMark.Length + 1;
            while (slice.Text[i].IsDigit())
            {
                i++;
            }

            return slice.Text[i] == tagClosingChar;
        }
    }
}