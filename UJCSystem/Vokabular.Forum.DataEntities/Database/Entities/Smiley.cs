using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Smiley {
        public int SmileyID { get; set; }
        public Board Board { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string Emoticon { get; set; }
        public byte SortOrder { get; set; }
    }
}
