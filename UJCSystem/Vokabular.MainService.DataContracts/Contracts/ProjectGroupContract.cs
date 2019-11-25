using System;
using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ProjectGroupContract
    {
        public int Id { get; set; }

        public DateTime CreateTime { get; set; }

        public IList<ProjectContract> Projects { get; set; }
    }
}