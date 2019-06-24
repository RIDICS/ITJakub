namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class RemoveUserFromRoleRequest
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}