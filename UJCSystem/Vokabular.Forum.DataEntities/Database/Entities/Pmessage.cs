using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class PMessage {
        public PMessage() {
			UserPMessages = new List<UserPMessage>();
        }
        public int PMessageID { get; set; }
        public User User { get; set; }
        public int? ReplyTo { get; set; }
        public DateTime Created { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int Flags { get; set; }
        public IList<UserPMessage> UserPMessages { get; set; }
    }
}
