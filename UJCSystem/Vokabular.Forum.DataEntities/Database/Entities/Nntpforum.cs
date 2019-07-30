using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class NntpForum {
        public NntpForum() {
			NntpTopics = new List<NntpTopic>();
        }
        public int NntpForumID { get; set; }
        public NntpServer NntpServer { get; set; }
        public Forum Forum { get; set; }
        public string GroupName { get; set; }
        public int LastMessageNo { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool Active { get; set; }
        public DateTime? DateCutOff { get; set; }
        public IList<NntpTopic> NntpTopics { get; set; }
    }
}
