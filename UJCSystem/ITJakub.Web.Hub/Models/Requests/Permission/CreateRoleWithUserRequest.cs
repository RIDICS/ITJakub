namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class CreateRoleWithUserRequest
    {
        public int UserId { get; set; }

        public string RoleName { get; set; }

        public string RoleDescription { get; set; }
    }
}