namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class RemoveProjectsFromRoleRequest
    {
        public int RoleId { get; set; }

        public long BookId { get; set; }
    }
}