using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Results
{
    [DataContract]
    public class AudioBookSearchResultContractList
    {
        [DataMember]
        public IList<AudioBookSearchResultContract> Results { get; set; }
    }

    [DataContract]
    public class AudioBookSearchResultContract: SearchResultContract
    {
        [DataMember]
        public IList<TrackContract> Tracks { get; set; }

        [DataMember]
        public IList<RecordingContract> FullBookRecordings { get; set; }
    }


    [DataContract]
    public class TrackContract
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public IList<TrackRecordingContract> Recordings { get; set; }
    }

    [DataContract]
    [KnownType(typeof(TrackRecordingContract))]
    public class RecordingContract
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public AudioTypeEnumContract AudioType { get; set; }

        [DataMember]
        public string MimeType { get; set; }
    }

    [DataContract]
    public class TrackRecordingContract : RecordingContract
    {
        [DataMember]
        public TimeSpan? Length { get; set; }
    }

    [DataContract]
    public enum AudioTypeEnumContract : byte
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Mp3 = 1,
        [EnumMember]
        Ogg = 2,
        [EnumMember]
        Wav = 3,
    }
}