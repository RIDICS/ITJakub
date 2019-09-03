﻿using System;
using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class NewPublicationViewModel
    {
        public IList<ResourceViewModel> AudioResourceList { get; set; }
        public IList<ResourceViewModel> ImageResourceList { get; set; }
        public IList<ResourceViewModel> TextResourceList { get; set; }
        public IList<GroupInfoViewModel> VisibilityForGroups { get; set; }
        public IList<BookTypeEnumContract> AvailableBookTypes { get; set; }
    }

    public class ResourceViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int ResourceVersionId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime Created { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
    }
    
    public class GroupInfoViewModel
    {
        public long GroupId { get; set; }
        public string Name { get; set; }
    }

    public class ResourcesViewModel
    {
        public ResourcesViewModel(ResourceTypeEnumContract resourceType, string title, IList<ResourceViewModel> resourceList)
        {
            ResourceType = resourceType;
            Title = title;
            ResourceList = resourceList;
        }

        public ResourceTypeEnumContract ResourceType { get; set; }
        public string Title { get; set; }
        public IList<ResourceViewModel> ResourceList { get; set; }
    }
}
