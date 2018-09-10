using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class PollGroupCluster {
        public PollGroupCluster() {
			Categories = new List<Category>();
			Forums = new List<Forum>();
			Polls = new List<Poll>();
			Topics = new List<Topic>();
        }
        public int PollGroupID { get; set; }
        public int UserID { get; set; }
        public int Flags { get; set; }
        public bool? IsBound { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<Forum> Forums { get; set; }
        public IList<Poll> Polls { get; set; }
        public IList<Topic> Topics { get; set; }
    }
}
