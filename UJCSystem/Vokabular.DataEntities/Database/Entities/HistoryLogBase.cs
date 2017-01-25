using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class HistoryLogBase
    {
        public virtual long Id { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }

    public class FullProjectImportLog : HistoryLogBase
    {
        public virtual string AdditionalDescription { get; set; }
    }
}