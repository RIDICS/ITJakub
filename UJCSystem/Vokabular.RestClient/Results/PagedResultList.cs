using System.Collections.Generic;

namespace Vokabular.RestClient.Results
{
    public class PagedResultList<T>
    {
        public List<T> List { get; set; }

        public int TotalCount { get; set; }
    }
}
