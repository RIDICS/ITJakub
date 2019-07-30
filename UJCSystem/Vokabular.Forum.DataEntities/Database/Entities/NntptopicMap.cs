using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class NntptopicMap : ClassMapping<NntpTopic> {
        
        public NntptopicMap() {
			Table("yaf_NntpTopic");
			Schema("dbo");
			Lazy(false);
			Id(x => x.NntpTopicID, map => map.Generator(Generators.Identity));
			Property(x => x.Thread, map => map.NotNullable(true));
			ManyToOne(x => x.NntpForum, map => { map.Column("NntpForumID"); map.Cascade(Cascade.None); });

			ManyToOne(x => x.Topic, map => 
			{
				map.Column("TopicID");
				map.PropertyRef("TopicID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
