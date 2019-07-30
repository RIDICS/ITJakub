using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class UserPMessage {
        public int UserPMessageID { get; set; }
        public User User { get; set; }
        public PMessage PMessage { get; set; }
        public int Flags { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsInOutbox { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsReply { get; set; }
    }
}
