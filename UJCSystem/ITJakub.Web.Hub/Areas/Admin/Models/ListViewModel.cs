using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ListViewModel<T>
    {
        public List<T> List { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Start { get; set; }
        public string SearchQuery { get; set; }
    }
}