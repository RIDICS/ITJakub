using System;
using System.IO;
using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts.AudioBooks
{
    [MessageContract]
    public class FileDataContract
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string MimeType { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream FileData { get; set; }
    }


    [MessageContract]
    public class AudioTrackContract
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string MimeType { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public TimeSpan? Lenght { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream FileData { get; set; }
    }
}