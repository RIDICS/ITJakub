namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class CreateGroupWithUserRequest
    {
        public int UserId { get; set; }

        public string GroupName { get; set; }

        public string GroupDescription { get; set; }
    }
}