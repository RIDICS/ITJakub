using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    public class FavoriteCategoryContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public IList<FavoriteBaseInfoContract> FavoriteInfo { get; set; }
    }
}