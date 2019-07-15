namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class AddSpecialPermissionsToRoleRequest
    {
        public int RoleId { get; set; }

        public int SpecialPermissionId { get; set; }
    }
}