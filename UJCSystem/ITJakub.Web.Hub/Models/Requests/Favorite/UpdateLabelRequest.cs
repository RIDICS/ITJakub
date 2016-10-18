namespace ITJakub.Web.Hub.Models.Requests.Favorite
{
    public class UpdateLabelRequest
    {
        public long LabelId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}