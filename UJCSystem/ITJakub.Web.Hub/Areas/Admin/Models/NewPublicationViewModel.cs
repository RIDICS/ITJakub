using System.Collections.Generic;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class NewPublicationViewModel
    {
        public IList<ResourceViewModel> ResourceList { get; set; }
        public IList<GroupInfoViewModel> VisibilityForGroups { get; set; }
    }

    public class ResourceViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IList<VersionNumberViewModel> VersionList { get; set; }
    }

    public class VersionNumberViewModel
    {
        public long ResourceVersionId { get; set; }
        public int VersionNumber { get; set; }
    }

    public class GroupInfoViewModel
    {
        public long GroupId { get; set; }
        public string Name { get; set; }
    }

    public class ResourcesViewModel
    {
        public ResourcesViewModel(ProjectResourceType resourceType, string title, IList<ResourceViewModel> resourceList)
        {
            Title = title;
            ResourceList = resourceList;
        }

        public ProjectResourceType ResourceType { get; set; }
        public string Title { get; set; }
        public IList<ResourceViewModel> ResourceList { get; set; }
    }
}
