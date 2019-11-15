namespace Vokabular.FulltextService.Core.Helpers.Validators
{
    public class TextValidator : ITextValidator
    {
        public ValidationResult Validate(string text)
        {
            // Add any required validations here

            return new ValidationResult { IsValid = true };
        }
    }
}