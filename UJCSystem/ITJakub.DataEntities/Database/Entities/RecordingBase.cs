using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public abstract class RecordingBase : IEquatable<RecordingBase>
    {
        public virtual long Id { get; set; }
        
        public virtual string FileName { get; set; }

        public virtual AudioType AudioType { get; set; }

        public virtual string MimeType { get; set; }

        public virtual bool Equals(RecordingBase other)
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
            return Equals((RecordingBase) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }



    public class TrackRecording : RecordingBase
    {
        public virtual TimeSpan? Length { get; set; }

        public virtual Track Track { get; set; }
    }

    public class FullBookRecording : RecordingBase
    {
        public virtual BookVersion BookVersion { get; set; }
    }

    public enum AudioType :byte
    {
        Unknown = 0,
        Mp3 = 1,
        Ogg = 2,
        Wav = 3,
        
    }
}