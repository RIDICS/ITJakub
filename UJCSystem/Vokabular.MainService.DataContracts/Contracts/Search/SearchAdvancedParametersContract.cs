using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class SearchAdvancedParametersContract
    {
        public bool IncludeAdditionalMetadata { get; set; }

        public List<ProjectTypeContract> AdditionalProjectTypes { get; set; }
    }
}
