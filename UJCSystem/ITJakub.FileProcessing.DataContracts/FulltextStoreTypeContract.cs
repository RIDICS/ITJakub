using System.Runtime.Serialization;

namespace ITJakub.FileProcessing.DataContracts
{
    [DataContract]
    public enum FulltextStoreTypeContract : short
    {
        [EnumMember]
        ExistDb = 0,

        [EnumMember]
        ElasticSearch = 1,
    }
}