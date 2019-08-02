using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class MessageReported {
        public MessageReported() {
			MessageReportedAudits = new List<MessageReportedAudit>();
        }
        public int MessageID { get; set; }
        public string Message { get; set; }
        public bool? Resolved { get; set; }
        public int? ResolvedBy { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public IList<MessageReportedAudit> MessageReportedAudits { get; set; }
    }
}
