using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ReplaceWord {
        public int ID { get; set; }
        public int BoardId { get; set; }
        public string BadWord { get; set; }
        public string GoodWord { get; set; }
    }
}
