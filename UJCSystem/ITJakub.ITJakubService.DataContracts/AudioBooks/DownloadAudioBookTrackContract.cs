using System.Runtime.Serialization;
using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts.AudioBooks
{
    [MessageContract]
    public class DownloadAudioBookTrackContract
    {
        [MessageHeader(MustUnderstand = true)]
        public long BookId { get; set; }
               

        [MessageHeader(MustUnderstand = true)]
        public int TrackPosition { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public AudioTypeContract RequestedAudioType { get; set; }

    }
}