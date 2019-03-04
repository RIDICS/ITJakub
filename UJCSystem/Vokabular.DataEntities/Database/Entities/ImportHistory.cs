using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ImportHistory : IEquatable<ImportHistory>
    {
        public virtual int Id { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual ImportStatusEnum Status { get; set; }

        public virtual string Message { get; set; }

        public virtual ExternalResource ExternalResource { get; set; }

        public virtual User UpdatedByUser { get; set; }

        public virtual IList<ImportMetadata> ImportMetadata { get; set; }

        public virtual bool Equals(ImportHistory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ImportHistory)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}