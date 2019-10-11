using System.Runtime.Serialization;

namespace ITJakub.FileProcessing.DataContracts
{
    [DataContract]
    public enum ProjectTypeContract : short
    {
        [EnumMember]
        Research = 0,

        [EnumMember]
        Community = 1,

        [EnumMember]
        Bibliography = 2,
    }
}