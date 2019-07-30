using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class GroupmedalMap : ClassMapping<GroupMedal> {
        
        public GroupmedalMap() {
			Table("yaf_GroupMedal");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.GroupID, m => m.Column("GroupID"));
					compId.Property(x => x.MedalID, m => m.Column("MedalID"));
				});
			Property(x => x.Message);
			Property(x => x.Hide, map => map.NotNullable(true));
			Property(x => x.OnlyRibbon, map => map.NotNullable(true));
			Property(x => x.SortOrder, map => map.NotNullable(true));
			ManyToOne(x => x.Group, map => 
			{
				map.Column("GroupID");
				map.PropertyRef("GroupID");
				map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });

			ManyToOne(x => x.Medal, map =>
            {
                map.Column("MedalID");
                map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });

        }
    }
}
