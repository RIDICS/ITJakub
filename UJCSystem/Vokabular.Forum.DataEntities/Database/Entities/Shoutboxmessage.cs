using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ShoutboxMessage {
        public int ShoutBoxMessageID { get; set; }
        public int BoardId { get; set; }
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string IP { get; set; }
    }
}
