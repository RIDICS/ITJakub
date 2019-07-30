using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ThankMap : ClassMapping<Thank> {
        
        public ThankMap() {
			Table("yaf_Thanks");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ThanksID, map => map.Generator(Generators.Identity));
			Property(x => x.ThanksToUserID, map => map.NotNullable(true));
			Property(x => x.MessageID, map => map.NotNullable(true));
			Property(x => x.ThanksDate, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("ThanksFromUserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
