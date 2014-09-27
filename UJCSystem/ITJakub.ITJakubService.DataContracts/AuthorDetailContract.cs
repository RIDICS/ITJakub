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
        public  int Id { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public  TextTypeEnum TextType { get; set; }
    }

    [DataContract]
    public enum TextTypeEnum
    {
        [EnumMember]
        Other,
        [EnumMember]
        FirstName,
        [EnumMember]
        LastName,
        [EnumMember]
        MiddleName,
        [EnumMember]
        BirthPlace,
        [EnumMember]
        NickName,
        [EnumMember]
        Numeral
    }
}