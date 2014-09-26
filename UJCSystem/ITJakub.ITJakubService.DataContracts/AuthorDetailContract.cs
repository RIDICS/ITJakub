using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class AuthorDetailContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public IEnumerable<AuthorInfoContract> AuthorInfos { get; set; }
    }

    [DataContract]
    public class AuthorInfoContract
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public  string Text { get; set; }
        [DataMember]
        public  int TextType { get; set; } //TODO should be enum
    }
}