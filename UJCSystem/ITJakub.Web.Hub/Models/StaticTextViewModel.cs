using ITJakub.Web.Hub.Models.Type;

namespace ITJakub.Web.Hub.Models
{
    public class StaticTextViewModel
    {
        public bool IsRecordExists { get; set; }

        public StaticTextFormatType Format { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }
    }
}