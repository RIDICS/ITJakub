using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ForumaccessMap : ClassMapping<ForumAccess> {
        
        public ForumaccessMap() {
			Table("yaf_ForumAccess");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.ManyToOne(x => x.Group, m => m.Column("GroupID"));
					compId.ManyToOne(x => x.Forum, m => m.Column("ForumID"));
				});

			ManyToOne(x => x.AccessMask, map => { map.Column("AccessMaskID"); map.Cascade(Cascade.None); });

        }
    }
}
