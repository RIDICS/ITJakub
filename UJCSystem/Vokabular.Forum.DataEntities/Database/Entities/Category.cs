using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Category {
        public Category() {
			Forums = new List<Forum>();
        }
        public int CategoryID { get; set; }
        public Board Board { get; set; }
        public PollGroupCluster PollGroupCluster { get; set; }
        public string Name { get; set; }
        public string CategoryImage { get; set; }
        public short SortOrder { get; set; }
        public IList<Forum> Forums { get; set; }
    }
}
