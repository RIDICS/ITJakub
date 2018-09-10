using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Buddy {
        public int ID { get; set; }
        public User User { get; set; }
        public int ToUserID { get; set; }
        public bool Approved { get; set; }
        public DateTime Requested { get; set; }
    }
}
