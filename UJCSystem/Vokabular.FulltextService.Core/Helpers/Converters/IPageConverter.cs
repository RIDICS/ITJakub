using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Helpers.Converters
{
    public interface IPageWithHtmlTagsCreator
    {
        string CreatePage(string textResourcePageText, TextFormatEnumContract formatValue);
    }
}