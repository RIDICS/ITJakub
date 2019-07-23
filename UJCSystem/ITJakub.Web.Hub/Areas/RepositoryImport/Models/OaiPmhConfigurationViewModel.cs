using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Models
{
    public class OaiPmhConfigurationViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "AdminMails")] 
        public string[] AdminMails { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Description")] 
        public string Description { get; set; }
        
        [DataType(DataType.Text)]
        [Display(Name = "Name")] 
        public string Name { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Url")] 
        public string Url { get; set; }
       
        public IList<MetadataFormatContract> MetadataFormats { get; set; }
        public IList<SetContract> Sets { get; set; }
    }
}
