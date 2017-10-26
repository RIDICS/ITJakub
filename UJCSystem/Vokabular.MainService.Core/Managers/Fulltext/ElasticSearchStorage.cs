using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public class ElasticSearchStorage : IFulltextStorage
    {
        public ProjectType ProjectType => ProjectType.Community;

        public string GetPageText(TextResource textResource, TextFormatEnumContract format)
        {
            throw new System.NotImplementedException();
        }
    }
}