using System;
using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class NewPublicationViewModel
    {
        public long ProjectId { get; set; }
        public IList<ResourceTypeViewModel> ResourceTypes { get; set; }
        public string Comment { get; set; }
        public IList<GroupInfoViewModel> VisibilityForGroups { get; set; }
        public IList<BookTypeEnumContract> AvailableBookTypes { get; set; }
        public BookTypeEnumContract DefaultBookType { get; set; }
        public IList<SelectableBookType> PublishBookTypes { get; set; }
    }

    public class ResourceViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ResourceVersionId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime Created { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SelectableBookType
    {
        public BookTypeEnumContract BookType { get; set; }
        public bool IsSelected { get; set; }
    }


    public class GroupInfoViewModel
    {
        public long GroupId { get; set; }
        public string Name { get; set; }
    }

    public class ResourceTypeViewModel
    {
        public string Title { get; set; }
        public ResourceTypeEnumContract ResourceType { get; set; }
        public IList<ResourceViewModel> ResourceList { get; set; }
    }
}
