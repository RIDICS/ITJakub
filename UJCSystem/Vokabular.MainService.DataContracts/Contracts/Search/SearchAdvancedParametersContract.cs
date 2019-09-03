using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class SearchAdvancedParametersContract
    {
        public bool IncludeAdditionalProjectTypes { get; set; }

        public List<ProjectTypeContract> AdditionalProjectTypes { get; set; }
    }
}
