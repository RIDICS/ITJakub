using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class UserpmessageMap : ClassMapping<UserPMessage> {
        
        public UserpmessageMap() {
			Table("yaf_UserPMessage");
			Schema("dbo");
			Lazy(false);
			Id(x => x.UserPMessageID, map => map.Generator(Generators.Identity));
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.IsRead);
			Property(x => x.IsInOutbox);
			Property(x => x.IsArchived);
			Property(x => x.IsDeleted);
			Property(x => x.IsReply, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.PMessage, map => 
			{
				map.Column("PMessageID");
				map.PropertyRef("PMessageID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
