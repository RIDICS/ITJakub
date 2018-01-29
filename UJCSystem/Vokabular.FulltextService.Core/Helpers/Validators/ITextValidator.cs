namespace Vokabular.FulltextService.Core.Helpers.Validators
{
    public interface ITextValidator
    {
        ValidationResult Validate(string text);
    }
}