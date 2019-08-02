using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Thank {
        public int ThanksID { get; set; }
        public User User { get; set; }
        public int ThanksToUserID { get; set; }
        public int MessageID { get; set; }
        public DateTime ThanksDate { get; set; }
    }
}
