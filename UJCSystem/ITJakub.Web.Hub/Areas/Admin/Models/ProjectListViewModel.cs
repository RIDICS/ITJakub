using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectListViewModel
    {
        public List<ProjectItemViewModel> List { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Start { get; set; }
    }
}