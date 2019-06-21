namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class RemoveSpecialPermissionsFromGroupRequest
    {
        public int GroupId { get; set; }

        public int SpecialPermissionId { get; set; }
    }
}