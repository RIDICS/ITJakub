using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.Shared.Contracts.News
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
        Combined = 0,

        [EnumMember]
        Web = 1,

        [EnumMember]
        MobileApps = 2,

    }
}