﻿using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Helpers;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectWorkMetadataViewModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string AuthorsText { get; set; }
        public string RelicAbbreviation { get; set; }
        public string SourceAbbreviation { get; set; }

        public List<LiteraryOriginalContract> AllLiteraryOriginalList { get; set; }
        public List<ResponsibleTypeViewModel> AllResponsibleTypeList { get; set; }
        
        public HashSet<int> SelectedLiteraryOriginalIds { get; set; }
        public List<OriginalAuthorContract> Authors { get; set; }
        public List<ProjectResponsiblePersonContract> ResponsiblePersons { get; set; }

        public string PublishPlace { get; set; }
        public string PublishDate { get; set; }
        public string PublisherText { get; set; }
        public string PublisherEmail { get; set; }
        public string Copyright { get; set; }
        public string BiblText { get; set; }
        public string OriginDate { get; set; }
        public DateTime? NotBefore { get; set; }
        public DateTime? NotAfter { get; set; }

        public string ManuscriptIdno { get; set; } //Signature
        public string ManuscriptSettlement { get; set; }
        public string ManuscriptCountry { get; set; }
        public string ManuscriptRepository { get; set; }
        public string ManuscriptExtent { get; set; }

        public DateTime? LastModification { get; set; }

        public string LiteraryOriginalText
        {
            get
            {
                return LiteraryOriginalTextConverter.GetLiteraryOriginalText(ManuscriptCountry, ManuscriptSettlement, ManuscriptRepository, ManuscriptIdno,
                    ManuscriptExtent);
            }
        }

        public ResponsibleTypeEnumViewModel ResponsibleTypeEnumEmpty { get; set; }
    }
}
