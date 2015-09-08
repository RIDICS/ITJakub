using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public class SelectedCategoriesContract
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public IList<SelectedItemContract> SelectedItems { get; set; }

        [DataMember]
        public IList<SelectedItemContract> SelectedCategories { get; set; }
    }

    [DataContract]
    public class SelectedItemContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
