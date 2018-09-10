using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class PollvoteMap : ClassMapping<PollVote> {
        
        public PollvoteMap() {
			Table("yaf_PollVote");
			Schema("dbo");
			Lazy(false);
			Id(x => x.PollVoteID, map => map.Generator(Generators.Identity));
			Property(x => x.UserID);
			Property(x => x.RemoteIP);
			Property(x => x.ChoiceID);
			ManyToOne(x => x.Poll, map => 
			{
				map.Column("PollID");
				map.PropertyRef("PollID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
