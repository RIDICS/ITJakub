using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class UsermedalMap : ClassMapping<UserMedal> {
        
        public UsermedalMap() {
			Table("yaf_UserMedal");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.UserID, m => m.Column("UserID"));
					compId.Property(x => x.MedalID, m => m.Column("MedalID"));
				});
			Property(x => x.Message);
			Property(x => x.Hide, map => map.NotNullable(true));
			Property(x => x.OnlyRibbon, map => map.NotNullable(true));
			Property(x => x.SortOrder, map => map.NotNullable(true));
			Property(x => x.DateAwarded, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Medal, map => { map.Column("MedalID"); map.Cascade(Cascade.None); });

        }
    }
}
