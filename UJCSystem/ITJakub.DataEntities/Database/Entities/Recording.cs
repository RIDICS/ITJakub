using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public class Recording : IEquatable<Recording>
    {
        protected virtual long Id { get; set; }

        public virtual Track Track { get; set; }

        public virtual TimeSpan Lenght { get; set; }

        public virtual string FileName { get; set; }

        public virtual AudioType AudioType { get; set; }

        public virtual string MimeType { get; set; }

        public virtual bool Equals(Recording other)
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
            return Equals((Recording) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }


    public enum AudioType
    {
        Mp3,
        Ogg,
        Wav,
    }
}