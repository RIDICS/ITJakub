using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks
{
    [MessageContract]    
    public class DownloadWholeBookContract
    {
        [MessageHeader(MustUnderstand = true)]
        public long BookId { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public AudioTypeContract RequestedAudioType { get; set; }
    }
}