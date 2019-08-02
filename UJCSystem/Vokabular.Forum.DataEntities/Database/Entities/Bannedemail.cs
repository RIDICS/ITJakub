using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class BannedEmail {
        public int ID { get; set; }
        public Board Board { get; set; }
        public string Mask { get; set; }
        public DateTime Since { get; set; }
        public string Reason { get; set; }
    }
}
