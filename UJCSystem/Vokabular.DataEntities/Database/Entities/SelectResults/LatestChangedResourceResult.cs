using System;

namespace Vokabular.DataEntities.Database.Entities.SelectResults
{
    public class LatestChangedResourceResult
    {
        public long ProjectId { get; set; }

        public DateTime CreateTime { get; set; }

        public int CreatedByUserId { get; set; }
    }
}