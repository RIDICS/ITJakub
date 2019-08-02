using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class PollvoterefuseMap : ClassMapping<PollVoteRefuse> {
        
        public PollvoterefuseMap() {
			Table("yaf_PollVoteRefuse");
			Schema("dbo");
			Lazy(false);
			Property(x => x.RefuseID, map => map.NotNullable(true));
			Property(x => x.PollID, map => map.NotNullable(true));
			Property(x => x.UserID);
			Property(x => x.RemoteIP);
        }
    }
}
