namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class AddProjectsToRoleRequest
    {
        public int RoleId { get; set; }

        public long BookId {get; set; }

        public bool ShowPublished { get; set; }
        public bool ReadProject { get; set; }
        public bool AdminProject { get; set; }
        public bool EditProject { get; set; }
    }
}