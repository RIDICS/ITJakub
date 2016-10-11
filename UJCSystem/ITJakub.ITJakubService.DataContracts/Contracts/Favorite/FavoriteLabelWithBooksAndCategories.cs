using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    public class FavoriteLabelWithBooksAndCategories
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public IList<long> BookIdList { get; set; }

        [DataMember]
        public IList<int> CategoryIdList { get; set; }
    }
}
