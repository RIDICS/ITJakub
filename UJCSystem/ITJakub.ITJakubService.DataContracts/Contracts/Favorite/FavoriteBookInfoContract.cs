using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    public class FavoriteBookInfoContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public IList<FavoriteBaseInfoContract> FavoriteInfo { get; set; }
    }
}
