using System.Text;
using System.Text.RegularExpressions;

namespace Vokabular.FulltextService.Core.Helpers.Validators
{
    public class TextValidator : ITextValidator
    {
        private readonly string StartingIdPattern = @"([^\\]|\\\\)\$([^\%]*)\%";
        private readonly string EndingIdPattern = @"\%([^\$]*)\$";
        private readonly string ContentPattern = @"[^\%]*";
        private readonly string MarksNoMatchError = "Starting and ending mark does not match. Maybe overlaping comments?";
        private readonly string MarkMissingError = "Comment marks not in pairs";

        public ValidationResult Validate(string text)
        {
            return ValidateCommentMarks(text);

        }

        private ValidationResult ValidateCommentMarks(string text)
        {
            var startingMarksCount = Regex.Matches(text, StartingIdPattern).Count;
            var endingMarksCount = Regex.Matches(text, EndingIdPattern).Count;

            if (startingMarksCount != endingMarksCount)
            {
                return new ValidationResult{ IsValid = false, ErrorMessage = MarkMissingError };
            }

            string pattern = $"{StartingIdPattern}{ContentPattern}{EndingIdPattern}";
            var matches = Regex.Matches(text, pattern);

            foreach (Match match in matches)
            {
                var startingId = match.Groups[2].Value;
                var endingId = match.Groups[3].Value;

                if (!startingId.Equals(endingId))
                {
                    return new ValidationResult { IsValid = false, ErrorMessage = MarksNoMatchError };
                }
            }

            return new ValidationResult { IsValid = true };
        }
    }
}