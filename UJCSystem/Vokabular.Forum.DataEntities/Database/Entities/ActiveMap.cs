using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ActiveMap : ClassMapping<Active> {
        
        public ActiveMap() {
			Table("yaf_Active");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.SessionID, m => m.Column("SessionID"));
					compId.Property(x => x.BoardID, m => m.Column("BoardID"));
				});
			Property(x => x.IP, map => map.NotNullable(true));
			Property(x => x.Login, map => map.NotNullable(true));
			Property(x => x.LastActive, map => map.NotNullable(true));
			Property(x => x.Location, map => map.NotNullable(true));
			Property(x => x.Browser);
			Property(x => x.Platform);
			Property(x => x.Flags);
			Property(x => x.ForumPage);
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Forum, map => 
			{
				map.Column("ForumID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Topic, map => 
			{
				map.Column("TopicID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

        }
    }
}
