using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class SmileyMap : ClassMapping<Smiley> {
        
        public SmileyMap() {
			Table("yaf_Smiley");
			Schema("dbo");
			Lazy(false);
			Id(x => x.SmileyID, map => map.Generator(Generators.Identity));
			Property(x => x.Code, map => { map.NotNullable(true); map.Unique(true); });
			Property(x => x.Icon, map => map.NotNullable(true));
			Property(x => x.Emoticon);
			Property(x => x.SortOrder, map => map.NotNullable(true));
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

        }
    }
}
