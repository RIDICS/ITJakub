using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class MessagereportedMap : ClassMapping<MessageReported> {
        
        public MessagereportedMap() {
			Table("yaf_MessageReported");
			Schema("dbo");
			Lazy(false);
			Id(x => x.MessageID, map => map.Generator(Generators.Assigned));
			Property(x => x.Message);
			Property(x => x.Resolved);
			Property(x => x.ResolvedBy);
			Property(x => x.ResolvedDate);
			Bag(x => x.MessageReportedAudits, colmap =>  { colmap.Key(x => x.Column("MessageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
