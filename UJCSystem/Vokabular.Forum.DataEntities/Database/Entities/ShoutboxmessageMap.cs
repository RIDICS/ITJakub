using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ShoutboxmessageMap : ClassMapping<ShoutboxMessage> {
        
        public ShoutboxmessageMap() {
			Table("yaf_ShoutboxMessage");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ShoutBoxMessageID, map => map.Generator(Generators.Identity));
			Property(x => x.BoardId, map => map.NotNullable(true));
			Property(x => x.UserID);
			Property(x => x.UserName, map => map.NotNullable(true));
			Property(x => x.UserDisplayName, map => map.NotNullable(true));
			Property(x => x.Message);
			Property(x => x.Date, map => map.NotNullable(true));
			Property(x => x.IP, map => map.NotNullable(true));
        }
    }
}
