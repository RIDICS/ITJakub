using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class TopicreadtrackingMap : ClassMapping<TopicReadTracking> {
        
        public TopicreadtrackingMap() {
			Table("yaf_TopicReadTracking");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.UserID, m => m.Column("UserID"));
					compId.Property(x => x.TopicID, m => m.Column("TopicID"));
				});
			Property(x => x.LastAccessDate, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });

			ManyToOne(x => x.Topic, map => 
			{
				map.Column("TopicID");
				map.PropertyRef("TopicID");
				map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });

        }
    }
}
