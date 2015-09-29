using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities
{
    public abstract class SpecialPermission : IEquatable<SpecialPermission>
    {
        public virtual int Id { get; set; }

        public virtual IList<Group> Groups { get; set; }

        public virtual bool Equals(SpecialPermission other)
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
            return Equals((SpecialPermission) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class UploadBookPermission : SpecialPermission
    {
        public virtual bool CanUploadBook { get; set; }

    }

    public class ManagePermissionsPermission : SpecialPermission
    {
        public virtual bool CanManagePermissions { get; set; }
    }

    public class AddNewsPermission : SpecialPermission
    {
        public virtual bool CanAddNews { get; set; }
    }
}