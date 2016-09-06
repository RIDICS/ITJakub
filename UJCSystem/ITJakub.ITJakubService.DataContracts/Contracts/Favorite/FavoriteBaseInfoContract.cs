using System;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    [KnownType(typeof(FavoriteBaseDetailContract))]
    public class FavoriteBaseInfoContract
    {
        [DataMember]
        public int Id { get; set; }

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
}