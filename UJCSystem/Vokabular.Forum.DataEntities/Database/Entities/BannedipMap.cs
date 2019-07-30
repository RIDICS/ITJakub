using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class BannedipMap : ClassMapping<BannedIP> {
        
        public BannedipMap() {
			Table("yaf_BannedIP");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ID, map => map.Generator(Generators.Identity));
			Property(x => x.Mask, map => { map.NotNullable(true); map.Unique(true); });
			Property(x => x.Since, map => map.NotNullable(true));
			Property(x => x.Reason);
			Property(x => x.UserID);
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

        }
    }
}
