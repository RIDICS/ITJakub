using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Choice {
        public int ChoiceID { get; set; }
        public Poll Poll { get; set; }
        public string ChoiceText { get; set; }
        public int Votes { get; set; }
        public string ObjectPath { get; set; }
        public string MimeType { get; set; }
    }
}
