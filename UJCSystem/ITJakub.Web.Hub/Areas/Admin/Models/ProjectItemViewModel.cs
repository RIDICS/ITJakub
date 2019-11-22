using System;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectItemViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastEditDate { get; set; }
        public string CreateUser { get; set; }
        public string LastEditUser { get; set; }
        public string PublisherString { get; set; }
        public string LiteraryOriginalString { get; set; }
        public int PageCount { get; set; }
        public PermissionDataContract Permissions { get; set; }
        public TextTypeEnumContract TextType { get; set; }
    }
}
