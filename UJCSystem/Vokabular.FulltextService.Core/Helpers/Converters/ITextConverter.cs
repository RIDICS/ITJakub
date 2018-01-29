using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Helpers.Converters
{
    public interface ITextConverter
    {
        string Convert(string textResourceText, TextFormatEnumContract formatValue);


    }
}