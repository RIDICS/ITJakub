using System.Runtime.Serialization;

namespace Vokabular.MainService.DataContracts.Contracts.CardFile
{
    [DataContract]
    public class CardImageContract
    {
        [DataMember]
        public string Id { get; set; }
    }
}