using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class TopicstatusMap : ClassMapping<TopicStatus> {
        
        public TopicstatusMap() {
			Table("yaf_TopicStatus");
			Schema("dbo");
			Lazy(false);
			Id(x => x.TopicStatusID, map => map.Generator(Generators.Identity));
			Property(x => x.TopicStatusName, map => map.NotNullable(true));
			Property(x => x.BoardID, map => map.NotNullable(true));
			Property(x => x.defaultDescription, map => map.NotNullable(true));
        }
    }
}
