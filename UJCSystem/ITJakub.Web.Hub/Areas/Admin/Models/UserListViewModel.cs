using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class UserListViewModel
    {
        public List<UserDetailContract> List { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Start { get; set; }
    }
}