using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class CheckemailMap : ClassMapping<CheckEmail> {
        
        public CheckemailMap() {
			Table("yaf_CheckEmail");
			Schema("dbo");
			Lazy(false);
			Id(x => x.CheckEmailID, map => map.Generator(Generators.Identity));
			Property(x => x.Email, map => map.NotNullable(true));
			Property(x => x.Created, map => map.NotNullable(true));
			Property(x => x.Hash, map => { map.NotNullable(true); map.Unique(true); });
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
