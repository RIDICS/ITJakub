using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Extension {
        public int ExtensionID { get; set; }
        public Board Board { get; set; }
        public string ExtensionText { get; set; }
    }
}
