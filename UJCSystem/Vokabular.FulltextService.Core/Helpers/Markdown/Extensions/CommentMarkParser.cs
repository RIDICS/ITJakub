using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Options;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentMarkParser : InlineParser
    {
        private readonly char m_closingChar;
        private readonly char m_escapeChar;
        private readonly int m_idLenght = 36;
        private readonly char m_openingChar;

        public CommentMarkParser(IOptions<SpecialCharsOption> options)
        {
            m_escapeChar = options.Value.EscapeCharacter[0];
            m_openingChar = options.Value.CommentMarkOpening[0];
            m_closingChar = options.Value.CommentMarkClosing[0];

            OpeningCharacters = new[] {m_openingChar};
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (IsCurrentCharEscaped(slice) || !IsBeginningOfTag(slice, m_closingChar)) //Comment mark is escaped or its not beginning of tag
            {
                return false;
            }
            
            var start = slice.Start;
            slice.NextChar();

            var openingId = slice.Text.Substring(slice.Start, m_idLenght);
            string closingId;
            int endOfContext = start;
            do
            {
                slice.Start = slice.Start + m_idLenght;
                var currentChar = slice.NextChar();
                do
                {
                    while (currentChar != m_closingChar) //Finds next closingChar, checks if its start of closing tag and if its not escaped
                    {
                        currentChar = slice.NextChar();
                        endOfContext = slice.Start;
                        if (currentChar == '\0') return false; //End of file
                    }
                } while (!IsBeginningOfTag(slice, m_openingChar) && IsCurrentCharEscaped(slice));

                slice.NextChar();
                closingId = slice.Text.Substring(slice.Start, m_idLenght); 
                
            } while (!openingId.Equals(closingId));

            slice.Start = slice.Start + m_idLenght + 1;
            int inlineStart = processor.GetSourcePosition(start, out int line, out int column);

            var commentMark = new CommentMarkContainer()
            {
                Span =
                {
                    Start = inlineStart,
                    End = inlineStart + (slice.Start - start) + 2
                },
                Line = line,
                Column = column,
                CommentId = openingId,
            };
            inlineStart = processor.GetSourcePosition(start + m_idLenght + 2, out int line1, out int column1);
            commentMark.ChildList = new List<Inline>();
            commentMark.ChildList.Add(new CommentMark
            {
                Span =
                {
                    Start = inlineStart,
                    End = inlineStart + (endOfContext  - start - m_idLenght) + 1
                },
                Line = line1,
                Column = column1,
                CommentContext = new StringSlice(slice.Text, start + m_idLenght + 2, endOfContext - 1)
            });

            processor.Inline = commentMark;
            return true;
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
                return true;

            return false;
        }

        private bool IsBeginningOfTag(StringSlice slice, char commentMarkClosingChar)
        {
            if (slice.PeekCharExtra(m_idLenght + 1) != commentMarkClosingChar) //Not comment mark
                return false;
            return true;
        }
    }
}