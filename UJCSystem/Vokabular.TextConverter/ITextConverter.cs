using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.TextConverter
{
    public interface ITextConverter
    {
        string ConvertText(string text, TextFormatEnumContract outputFormat);
    }
}