namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class PermissionsConfigurationRequest
    {
        public long BookId {get; set; }

        public bool ShowPublished { get; set; }
        public bool ReadProject { get; set; }
        public bool AdminProject { get; set; }
        public bool EditProject { get; set; }
    }

    public class AddProjectsToGroupRequest
    {
        public int RoleId { get; set; }

        public PermissionsConfigurationRequest PermissionsConfiguration { get; set; }
    }
    
    public class AddProjectsToSingleUserGroupRequest
    {
        public string UserCode { get; set; }
        
        public PermissionsConfigurationRequest PermissionsConfiguration { get; set; }
    }
}