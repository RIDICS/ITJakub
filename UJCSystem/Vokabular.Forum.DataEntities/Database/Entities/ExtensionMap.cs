using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ExtensionMap : ClassMapping<Extension> {
        
        public ExtensionMap() {
			Table("yaf_Extension");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ExtensionID, map => map.Generator(Generators.Identity));
			Property(x => x.ExtensionText, map => map.NotNullable(true));
			ManyToOne(x => x.Board, map => { map.Column("BoardId"); map.Cascade(Cascade.None); });

        }
    }
}
