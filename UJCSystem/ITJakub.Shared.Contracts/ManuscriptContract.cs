using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class ManuscriptContract
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Settlement { get; set; }

        [DataMember]
        public string Idno { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string OriginDate { get; set; }

        [DataMember]
        public string Repository { get; set; }
    }
}