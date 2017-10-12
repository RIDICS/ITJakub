using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class TrackContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
        public int Position { get; set; }
        public long? ChapterId { get; set; }
        public long? BeginningPageId { get; set; }
    }

    public class TrackWithRecordingContract : TrackContract
    {
        public List<AudioContract> Recordings { get; set; }
    }
}