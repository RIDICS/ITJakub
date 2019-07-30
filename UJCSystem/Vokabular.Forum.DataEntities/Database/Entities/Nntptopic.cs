using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class NntpTopic {
        public int NntpTopicID { get; set; }
        public NntpForum NntpForum { get; set; }
        public Topic Topic { get; set; }
        public string Thread { get; set; }
    }
}
