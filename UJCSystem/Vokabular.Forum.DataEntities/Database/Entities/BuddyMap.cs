using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class BuddyMap : ClassMapping<Buddy> {
        
        public BuddyMap() {
			Table("yaf_Buddy");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ID, map => map.Generator(Generators.Identity));
			Property(x => x.ToUserID, map => { map.NotNullable(true); map.Unique(true); });
			Property(x => x.Approved, map => map.NotNullable(true));
			Property(x => x.Requested, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("FromUserID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
