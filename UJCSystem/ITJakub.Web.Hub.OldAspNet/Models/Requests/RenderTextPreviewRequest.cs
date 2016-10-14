using ITJakub.Web.Hub.Models.Type;

namespace ITJakub.Web.Hub.Models.Requests
{
    public class RenderTextPreviewRequest
    {
        public string Text { get; set; }

        public StaticTextFormatType InputTextFormat { get; set; }
    }
}