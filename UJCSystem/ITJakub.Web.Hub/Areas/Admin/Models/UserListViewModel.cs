using System.Collections.Generic;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class UserListViewModel
    {
        public List<UserDetailViewModel> List { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Start { get; set; }
    }
}