using System;
using System.Runtime.Serialization;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Favorites;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    public class FavoriteQueryContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public DateTime? CreateTime { get; set; }

        [DataMember]
        public string Query { get; set; }

        [DataMember]
        public BookTypeEnumContract BookType { get; set; }

        [DataMember]
        public QueryTypeEnumContract QueryType { get; set; }

        [DataMember]
        public FavoriteLabelContract FavoriteLabel { get; set; }
    }
}