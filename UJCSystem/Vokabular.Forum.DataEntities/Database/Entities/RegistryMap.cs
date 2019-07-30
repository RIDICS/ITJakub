using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class RegistryMap : ClassMapping<Registry> {
        
        public RegistryMap() {
			Table("yaf_Registry");
			Schema("dbo");
			Lazy(false);
			Id(x => x.RegistryID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => map.NotNullable(true));
			Property(x => x.Value);
			ManyToOne(x => x.Board, map => 
			{
				map.Column("BoardID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

        }
    }
}
