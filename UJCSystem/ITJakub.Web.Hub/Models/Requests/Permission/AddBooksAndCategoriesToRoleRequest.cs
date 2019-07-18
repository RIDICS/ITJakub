using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class AddBooksAndCategoriesToRoleRequest
    {
        public int RoleId { get; set; }

        public IList<long> BookIds {get; set; }

        public IList<int> CategoryIds { get; set; }
    }
}