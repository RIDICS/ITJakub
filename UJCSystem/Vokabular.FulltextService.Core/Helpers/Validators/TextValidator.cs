using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Options;

namespace Vokabular.FulltextService.Core.Helpers.Validators
{
    public class TextValidator : ITextValidator
    {
        private readonly string m_escapeChar;
        private readonly string m_commentMarkBegining;
        private readonly string m_commentMarkEnding;

        private string m_startingIdPattern;
        
        private readonly string MarksNoMatchError = "Starting and ending marks does not match. Maybe overlaping comments?";
       
        public TextValidator(IOptions<SpecialCharsOption> options)
        {
            m_escapeChar = options.Value.EscapedEscapeCharacter;
            m_commentMarkBegining = options.Value.EscapedCommentMarkBeginning;
            m_commentMarkEnding = options.Value.EscapedCommentMarkEnding;

            CreateRegexPatterns();
        }

        private void CreateRegexPatterns()
        {
            //m_startingIdPattern = $"(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_commentMarkBegining}([^{m_commentMarkEnding}]*){m_commentMarkEnding}"; 
            //m_endingIdPattern = $"(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_commentMarkEnding}([^{m_commentMarkBegining}]*){m_commentMarkBegining}";
            //m_contentPattern = $"(?:(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_escapeChar}{m_commentMarkEnding}|[^{m_commentMarkEnding}])+(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*";
            //m_pattern = $"{m_startingIdPattern}{m_contentPattern}{m_commentMarkEnding}([^{m_commentMarkBegining}]*){m_commentMarkBegining}";

            m_startingIdPattern = $"(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_commentMarkBegining}([0-9a-f-]{{36}}){m_commentMarkEnding}|(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_commentMarkEnding}([0-9a-f-]{{36}}){m_commentMarkBegining}"; 
            
        }

        public ValidationResult Validate(string text)
        {
            return ValidateCommentMarks(text);

        }

        private ValidationResult ValidateCommentMarks(string text)
        {
            var commentMarks = Regex.Matches(text, m_startingIdPattern);
            
            Stack<string> stack = new Stack<string>();

            foreach (Match startingMark in commentMarks)
            {
                var id = string.IsNullOrEmpty(startingMark.Groups[1].Value) ? startingMark.Groups[2].Value : startingMark.Groups[1].Value;
                if (stack.Count == 0)
                {
                    stack.Push(id);
                    continue;
                }
                if (stack.Peek().Equals(id))
                {
                    stack.Pop();
                }
                else
                {
                    stack.Push(id);
                }
            }

            if (stack.Count != 0)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = MarksNoMatchError };
            }
            return new ValidationResult { IsValid = true };
        }
    }
}