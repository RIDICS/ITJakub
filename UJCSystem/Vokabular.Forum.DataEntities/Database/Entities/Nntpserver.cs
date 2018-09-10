using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class NntpServer {
        public NntpServer() {
			NntpForums = new List<NntpForum>();
        }
        public int NntpServerID { get; set; }
        public Board Board { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? Port { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }
        public IList<NntpForum> NntpForums { get; set; }
    }
}
