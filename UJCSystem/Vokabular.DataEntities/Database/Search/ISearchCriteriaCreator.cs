using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Search
{
    public interface ISearchCriteriaCreator
    {
        Dictionary<string, object> Parameters { get; }
    }
}