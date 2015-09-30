using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.SqlServer.Entities
{
    public class TaskData : ITaskEntity, IEquatable<TaskData>
    {
        public virtual long Id { get; protected set; }

        public virtual Task Task { get; set; }

        public virtual Application Application { get; set; }

        public virtual string Data { get; set; }

        public virtual bool Equals(TaskData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TaskData) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}