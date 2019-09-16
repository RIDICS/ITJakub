namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class CreateEditionNoteRequest
    {
        public long ProjectId { get; set; }
        public long? OriginalVersionId { get; set; }
        public string Content { get; set; }
    }
}
