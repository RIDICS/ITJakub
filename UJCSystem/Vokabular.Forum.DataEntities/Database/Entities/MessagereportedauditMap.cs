using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class MessagereportedauditMap : ClassMapping<MessageReportedAudit> {
        
        public MessagereportedauditMap() {
			Table("yaf_MessageReportedAudit");
			Schema("dbo");
			Lazy(false);
			Property(x => x.LogID, map => map.NotNullable(true));
			Property(x => x.UserID);
			Property(x => x.Reported);
			Property(x => x.ReportedNumber, map => map.NotNullable(true));
			Property(x => x.ReportText);
			ManyToOne(x => x.MessageReported, map => { map.Column("MessageID"); map.Cascade(Cascade.None); });

        }
    }
}
