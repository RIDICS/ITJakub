using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    public class PageBookmarkContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string PageXmlId { get; set; }
        
        [DataMember]
        public int PagePosition { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public FavoriteLabelContract FavoriteLabel { get; set; }
    }
}