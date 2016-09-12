using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class RemoveSpecialPermissionsFromGroupRequest
    {
        public int GroupId { get; set; }

        public IList<int> SpecialPermissionIds { get; set; }
    }
}