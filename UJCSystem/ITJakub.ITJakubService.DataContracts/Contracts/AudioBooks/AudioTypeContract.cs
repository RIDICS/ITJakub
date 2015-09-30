using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks
{
    [DataContract]
    public enum AudioTypeContract:byte
    {
        [EnumMember]Mp3 = 1,
        [EnumMember]Ogg = 2,
        [EnumMember]Wav = 3,
    }
}