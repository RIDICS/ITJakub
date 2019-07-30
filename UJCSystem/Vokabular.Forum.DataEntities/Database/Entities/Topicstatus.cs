using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class TopicStatus {
        public int TopicStatusID { get; set; }
        public string TopicStatusName { get; set; }
        public int BoardID { get; set; }
        public string defaultDescription { get; set; }
    }
}
