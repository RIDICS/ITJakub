namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class AddProjectsToRoleRequest
    {
        public int RoleId { get; set; }

        public long BookId {get; set; }
    }
}