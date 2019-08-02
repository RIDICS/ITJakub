using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class WatchTopic {
        public int WatchTopicID { get; set; }
        public Topic Topic { get; set; }
        public User User { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastMail { get; set; }
    }
}
