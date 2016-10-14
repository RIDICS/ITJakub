using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class AddSpecialPermissionsToGroupRequest
    {
        public int GroupId { get; set; }

        public IList<int> SpecialPermissionIds { get; set; }
    }
}