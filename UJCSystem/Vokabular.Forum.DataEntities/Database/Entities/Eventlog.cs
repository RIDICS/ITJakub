using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class EventLog {
        public int EventLogID { get; set; }
        public User User { get; set; }
        public DateTime EventTime { get; set; }
        public string UserName { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
    }
}
