using System;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ProjectMetadataContract
    {
        public string Editor { get; set; }
        public ProjectLiteraryOriginalContract LiteraryOriginal { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }
        public string LiteraryKind { get; set; }
        public string LiteraryGenre { get; set; }
        public DateTime LastModification { get; set; }
    }

    public class ProjectLiteraryOriginalContract
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Institution { get; set; }
        public string Signature { get; set; }
        public string Extent { get; set; }
    }
}
