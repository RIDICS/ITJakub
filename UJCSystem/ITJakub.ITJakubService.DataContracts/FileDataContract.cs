using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts
{
    [MessageContract]
    public class FileDataContract
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName { get; set; }


        [MessageBodyMember(Order = 1)]
        public Stream FileData { get; set; }        
    }

    [DataContract]
    public enum AudioTypeContract:byte
    {
        [EnumMember]Mp3 = 1,
        [EnumMember]Ogg = 2,
        [EnumMember]Wav = 3,
    }

}