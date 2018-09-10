using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class NntpforumMap : ClassMapping<NntpForum> {
        
        public NntpforumMap() {
			Table("yaf_NntpForum");
			Schema("dbo");
			Lazy(false);
			Id(x => x.NntpForumID, map => map.Generator(Generators.Identity));
			Property(x => x.GroupName, map => map.NotNullable(true));
			Property(x => x.LastMessageNo, map => map.NotNullable(true));
			Property(x => x.LastUpdate, map => map.NotNullable(true));
			Property(x => x.Active, map => map.NotNullable(true));
			Property(x => x.DateCutOff);
			ManyToOne(x => x.NntpServer, map => { map.Column("NntpServerID"); map.Cascade(Cascade.None); });

			ManyToOne(x => x.Forum, map => 
			{
				map.Column("ForumID");
				map.PropertyRef("ForumID");
				map.Cascade(Cascade.None);
			});

			Bag(x => x.NntpTopics, colmap =>  { colmap.Key(x => x.Column("NntpForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
