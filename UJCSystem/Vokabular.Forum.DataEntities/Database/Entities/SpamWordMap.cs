using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class SpamWordMap : ClassMapping<SpamWord> {
        
        public SpamWordMap() {
			Table("yaf_Spam_Words");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ID, map => map.Generator(Generators.Identity));
			Property(x => x.BoardId, map => map.NotNullable(true));
			Property(x => x.SpamWordText);
        }
    }
}
