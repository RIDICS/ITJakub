using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class SpamWord {
        public int ID { get; set; }
        public int BoardId { get; set; }
        public string SpamWordText { get; set; }
    }
}
