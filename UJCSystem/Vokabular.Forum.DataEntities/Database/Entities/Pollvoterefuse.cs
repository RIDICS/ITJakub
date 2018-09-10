using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class PollVoteRefuse {
        public int RefuseID { get; set; }
        public int PollID { get; set; }
        public int? UserID { get; set; }
        public string RemoteIP { get; set; }
    }
}
