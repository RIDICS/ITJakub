using System;
using System.Runtime.Serialization;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Favorites;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    [KnownType(typeof(FavoriteBaseDetailContract))]
    [KnownType(typeof(FavoriteFullInfoContract))]
    public class FavoriteBaseInfoContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Title { get; set; }
        
        [DataMember]
        public FavoriteTypeContract FavoriteType { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
    }

    [DataContract]
    public class FavoriteBaseDetailContract : FavoriteBaseInfoContract
    {
        [DataMember]
        public FavoriteLabelContract FavoriteLabel { get; set; }
    }

    [DataContract]
    public class FavoriteFullInfoContract : FavoriteBaseInfoContract
    {
        [DataMember]
        public BookIdContract Book { get; set; }

        [DataMember]
        public string PageXmlId { get; set; }

        [DataMember]
        public int PagePosition { get; set; }

        [DataMember]
        public CategoryContract Category { get; set; }

        [DataMember]
        public BookTypeEnumContract BookType { get; set; }

        [DataMember]
        public QueryTypeEnumContract QueryType { get; set; }

        [DataMember]
        public string Query { get; set; }
    }
}