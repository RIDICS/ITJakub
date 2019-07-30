using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class EventlogMap : ClassMapping<EventLog> {
        
        public EventlogMap() {
			Table("yaf_EventLog");
			Schema("dbo");
			Lazy(false);
			Id(x => x.EventLogID, map => map.Generator(Generators.Identity));
			Property(x => x.EventTime, map => map.NotNullable(true));
			Property(x => x.UserName);
			Property(x => x.Source, map => map.NotNullable(true));
			Property(x => x.Description, map => map.NotNullable(true));
			Property(x => x.Type, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

        }
    }
}
