using System;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectWorkMetadataViewModel
    {
        public string Editor { get; set; }
        public string LiteraryOriginal { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }
        public string LiteraryKind { get; set; }
        public string LiteraryGenre { get; set; }
        public DateTime LastModification { get; set; }
    }
}
