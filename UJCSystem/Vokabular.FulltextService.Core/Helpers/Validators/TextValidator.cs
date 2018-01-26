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
        private string m_endingIdPattern;
        private string m_contentPattern;
        private string m_pattern;

        private readonly string MarksNoMatchError = "Starting and ending mark does not match. Maybe overlaping comments?";
        private readonly string MarkMissingError = "Comment marks not in pairs";
        

        public TextValidator(IOptions<SpecialCharsOption> options)
        {
            m_escapeChar = options.Value.EscapedEscapeCharacter;
            m_commentMarkBegining = options.Value.EscapedCommentMarkBeginning;
            m_commentMarkEnding = options.Value.EscapedCommentMarkEnding;

            CreateRegexPatterns();
        }

        private void CreateRegexPatterns()
        {
            m_startingIdPattern = $"(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_commentMarkBegining}([^{m_commentMarkEnding}]*){m_commentMarkEnding}"; 
            m_endingIdPattern = $"(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_commentMarkEnding}([^{m_commentMarkBegining}]*){m_commentMarkBegining}";
            m_contentPattern = $"(?:(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*{m_escapeChar}{m_commentMarkEnding}|[^{m_commentMarkEnding}])+(?<!{m_escapeChar})(?:{m_escapeChar}{{2}})*";
            m_pattern = $"{m_startingIdPattern}{m_contentPattern}{m_commentMarkEnding}([^{m_commentMarkBegining}]*){m_commentMarkBegining}";
        }

        public ValidationResult Validate(string text)
        {
            return ValidateCommentMarks(text);

        }

        private ValidationResult ValidateCommentMarks(string text)
        {
            var startingMarksCount = Regex.Matches(text, m_startingIdPattern).Count;
            var endingMarksCount = Regex.Matches(text, m_endingIdPattern).Count;

            if (startingMarksCount != endingMarksCount)
            {
                return new ValidationResult{ IsValid = false, ErrorMessage = MarkMissingError };
            }

            
            var matches = Regex.Matches(text, m_pattern);

            foreach (Match match in matches)
            {
                var startingId = match.Groups[1].Value;
                var endingId = match.Groups[2].Value;

                if (!startingId.Equals(endingId))
                {
                    return new ValidationResult { IsValid = false, ErrorMessage = MarksNoMatchError };
                }
            }

            return new ValidationResult { IsValid = true };
        }
    }
}