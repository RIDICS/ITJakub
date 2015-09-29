using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.MobileContracts.News
{
    [DataContract]
    public class NewsSyndicationItemContract
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public NewsTypeContract ItemType { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string UserEmail { get; set; }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }
    }

    [DataContract]
    public enum NewsTypeContract
    {
        [EnumMember]
        Combined,

        [EnumMember]
        Web,

        [EnumMember]
        MobileApps,

    }
}