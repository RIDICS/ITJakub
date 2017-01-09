using System;
using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectWorkMetadataViewModel
    {
        public string Editor { get; set; }
        public ProjectWorkLiteraryOriginalViewModel LiteraryOriginal { get; set; }
        public string LiteraryOriginalText { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }
        public DateTime LastModification { get; set; }

        public List<PublisherContract> PublisherList { get; set; }
        public List<LiteraryKindContract> LiteraryKindList { get; set; }
        public List<LiteraryGenreContract> LiteraryGenreList { get; set; }
    }

    public class ProjectWorkLiteraryOriginalViewModel
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Institution { get; set; }
        public string Signature { get; set; }
        public string Extent { get; set; }
    }
}
