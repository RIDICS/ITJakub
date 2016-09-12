namespace ITJakub.Web.Hub.Models.Requests.Permission
{
    public class RemoveUserFromGroupRequest
    {
        public int UserId { get; set; }

        public int GroupId { get; set; }
    }
}