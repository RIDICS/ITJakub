using System;
using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectWorkMetadataViewModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }

        public ProjectWorkLiteraryOriginalViewModel LiteraryOriginal { get; set; }
        
        public List<PublisherContract> AllPublisherList { get; set; }
        public List<LiteraryKindContract> AllLiteraryKindList { get; set; }
        public List<LiteraryGenreContract> AllLiteraryGenreList { get; set; }
        // TODO add property IsSelected to PublisherViewModel, Kind, Genre

        public string PublishPlace { get; set; }
        public string PublishDate { get; set; }
        public string Copyright { get; set; }
        public string BiblText { get; set; }
        public string OriginDate { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }

        public string Editor { get; set; } //TODO remove
        public DateTime LastModification { get; set; }

        public string LiteraryOriginalText
        {
            get
            {
                return $"{LiteraryOriginal.Country}, {LiteraryOriginal.Settlement}, {LiteraryOriginal.Repository}, {LiteraryOriginal.Idno}, {LiteraryOriginal.Extent}";
            }
        }
    }

    public class ProjectWorkLiteraryOriginalViewModel
    {
        public string Idno { get; set; } //Signature
        public string Settlement { get; set; }
        public string Country { get; set; }
        public string Repository { get; set; }
        public string Extent { get; set; }
    }
}
