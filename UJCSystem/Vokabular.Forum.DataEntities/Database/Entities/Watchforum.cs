using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class WatchForum {
        public int WatchForumID { get; set; }
        public Forum Forum { get; set; }
        public User User { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastMail { get; set; }
    }
}
