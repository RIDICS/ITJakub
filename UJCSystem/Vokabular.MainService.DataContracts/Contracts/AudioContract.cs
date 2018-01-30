using System;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class AudioContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }

        public TimeSpan? Duration { get; set; }
        public string FileName { get; set; }
        public AudioTypeEnumContract AudioType { get; set; }
        public string MimeType { get; set; }
    }

    public class CreateAudioContract
    {
        public string Comment { get; set; }
        public long OriginalVersionId { get; set; }
        public long? ResourceTrackId { get; set; }
        public TimeSpan? Duration { get; set; }
        public string FileName { get; set; }
        //public AudioTypeEnumContract AudioType { get; set; }
        //public string MimeType { get; set; }
    }
}
