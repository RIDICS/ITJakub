using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities.SelectResults
{
    public class ListWithTotalCountResult<T>
    {
        public IList<T> List { get; set; }
        public int Count { get; set; }
    }
}
