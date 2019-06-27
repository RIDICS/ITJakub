using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ReputationvoteMap : ClassMapping<ReputationVote> {
        
        public ReputationvoteMap() {
			Table("yaf_ReputationVote");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.ReputationFromUserID, m => m.Column("ReputationFromUserID"));
					compId.Property(x => x.ReputationToUserID, m => m.Column("ReputationToUserID"));
				});
			Property(x => x.VoteDate, map => map.NotNullable(true));
			ManyToOne(x => x.ReputationFromUser, map => 
			{
				map.Column("ReputationFromUserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });

			ManyToOne(x => x.ReputationToUser, map => 
			{
				map.Column("ReputationToUserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });

        }
    }
}
