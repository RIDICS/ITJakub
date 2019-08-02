using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class WatchforumMap : ClassMapping<WatchForum> {
        
        public WatchforumMap() {
			Table("yaf_WatchForum");
			Schema("dbo");
			Lazy(false);
			Id(x => x.WatchForumID, map => map.Generator(Generators.Identity));
			Property(x => x.Created, map => map.NotNullable(true));
			Property(x => x.LastMail);
			ManyToOne(x => x.Forum, map => 
			{
				map.Column("ForumID");
				map.PropertyRef("ForumID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
