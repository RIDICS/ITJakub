namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class AddUserToRoleRequest
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}