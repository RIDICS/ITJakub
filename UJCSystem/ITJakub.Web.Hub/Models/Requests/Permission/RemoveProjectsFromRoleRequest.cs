using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class RemoveProjectsFromRoleRequest
    {
        public int RoleId { get; set; }

        public IList<long> BookIds { get; set; }
    }
}