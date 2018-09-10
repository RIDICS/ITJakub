using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class PollVote {
        public int PollVoteID { get; set; }
        public Poll Poll { get; set; }
        public int? UserID { get; set; }
        public string RemoteIP { get; set; }
        public int? ChoiceID { get; set; }
    }
}
