using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.DataContracts.Data
{
    public class ProjectListData
    {
        public List<ProjectContract> List { get; set; }
        public int TotalCount { get; set; }
    }
}
