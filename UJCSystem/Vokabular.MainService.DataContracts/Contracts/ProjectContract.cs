using System;
using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ProjectContract
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ProjectTypeContract ProjectType { get; set; }
        //public string ExternalId { get; set; }
    }

    public class GetProjectContract : ProjectContract
    {
        public DateTime CreateTime { get; set; }
        public UserContract CreatedByUser { get; set; }
    }

    public class ProjectDetailContract : GetProjectContract
    {
        public ProjectMetadataContract LatestMetadata { get; set; }
        public int? PageCount { get; set; }
        public List<OriginalAuthorContract> Authors { get; set; }
        public List<ProjectResponsiblePersonContract> ResponsiblePersons { get; set; }
    }
}
