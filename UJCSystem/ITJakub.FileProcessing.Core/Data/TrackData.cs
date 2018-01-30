using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Data
{
    public class TrackData
    {
        public IList<TrackRecordingData> Recordings { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int Position { get; set; }
    }
}