using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class MessagehistoryMap : ClassMapping<MessageHistory> {
        
        public MessagehistoryMap() {
			Table("yaf_MessageHistory");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.MessageID, m => m.Column("MessageID"));
					compId.Property(x => x.Edited, m => m.Column("Edited"));
				});
			Property(x => x.MessageText, map => map.NotNullable(true));
			Property(x => x.IP, map => map.NotNullable(true));
			Property(x => x.EditedBy);
			Property(x => x.EditReason);
			Property(x => x.IsModeratorChanged, map => map.NotNullable(true));
			Property(x => x.Flags, map => map.NotNullable(true));
			ManyToOne(x => x.Message, map => 
			{
				map.Column("MessageID");
				map.PropertyRef("MessageID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
