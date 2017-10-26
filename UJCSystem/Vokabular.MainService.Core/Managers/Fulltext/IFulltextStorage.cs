using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public interface IFulltextStorage
    {
        ProjectType ProjectType { get; }
        string GetPageText(TextResource textResource, TextFormatEnumContract format);
    }
}
