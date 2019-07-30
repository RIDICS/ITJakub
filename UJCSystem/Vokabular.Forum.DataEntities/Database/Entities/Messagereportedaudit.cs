using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class MessageReportedAudit {
        public MessageReported MessageReported { get; set; }
        public int LogID { get; set; }
        public int? UserID { get; set; }
        public DateTime? Reported { get; set; }
        public int ReportedNumber { get; set; }
        public string ReportText { get; set; }
    }
}
