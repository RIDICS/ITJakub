using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Poll {
        public Poll() {
			Choices = new List<Choice>();
			PollVotes = new List<PollVote>();
        }
        public int PollID { get; set; }
        public PollGroupCluster PollGroupCluster { get; set; }
        public string Question { get; set; }
        public DateTime? Closes { get; set; }
        public int UserID { get; set; }
        public string ObjectPath { get; set; }
        public string MimeType { get; set; }
        public int Flags { get; set; }
        public bool? IsClosedBound { get; set; }
        public bool? AllowMultipleChoices { get; set; }
        public bool? ShowVoters { get; set; }
        public bool? AllowSkipVote { get; set; }
        public IList<Choice> Choices { get; set; }
        public IList<PollVote> PollVotes { get; set; }
    }
}
