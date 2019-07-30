using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class BannednameMap : ClassMapping<BannedName> {
        
        public BannednameMap() {
			Table("yaf_BannedName");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ID, map => map.Generator(Generators.Identity));
			Property(x => x.Mask, map => { map.NotNullable(true); map.Unique(true); });
			Property(x => x.Since, map => map.NotNullable(true));
			Property(x => x.Reason);
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

        }
    }
}
