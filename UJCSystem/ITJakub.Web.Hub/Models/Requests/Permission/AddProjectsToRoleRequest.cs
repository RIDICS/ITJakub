using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class AddProjectsToRoleRequest
    {
        public int RoleId { get; set; }

        public IList<long> BookIds {get; set; }
    }
}