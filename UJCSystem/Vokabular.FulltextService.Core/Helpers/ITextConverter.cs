using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Helpers
{
    public interface ITextConverter
    {
        string Convert(string textResourceText, TextFormatEnumContract formatValue);


    }
}