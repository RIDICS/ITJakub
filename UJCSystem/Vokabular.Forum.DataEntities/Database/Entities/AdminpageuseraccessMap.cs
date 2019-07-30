using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class AdminpageuseraccessMap : ClassMapping<AdminPageUserAccess> {
        
        public AdminpageuseraccessMap() {
			Table("yaf_AdminPageUserAccess");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.UserID, m => m.Column("UserID"));
					compId.Property(x => x.PageName, m => m.Column("PageName"));
				});
			ManyToOne(x => x.User, map =>
			{
				map.Column("UserID");
				map.Cascade(Cascade.None);
				map.Insert(false);
				map.Update(false);
			});

        }
    }
}
