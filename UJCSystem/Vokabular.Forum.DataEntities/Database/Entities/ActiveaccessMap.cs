using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ActiveaccessMap : ClassMapping<ActiveAccess> {
        
        public ActiveaccessMap() {
			Table("yaf_ActiveAccess");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.UserID, m => m.Column("UserID"));
					compId.Property(x => x.ForumID, m => m.Column("ForumID"));
				});
			Property(x => x.BoardID, map => map.NotNullable(true));
			Property(x => x.IsAdmin, map => map.NotNullable(true));
			Property(x => x.IsForumModerator, map => map.NotNullable(true));
			Property(x => x.IsModerator, map => map.NotNullable(true));
			Property(x => x.ReadAccess, map => map.NotNullable(true));
			Property(x => x.PostAccess, map => map.NotNullable(true));
			Property(x => x.ReplyAccess, map => map.NotNullable(true));
			Property(x => x.PriorityAccess, map => map.NotNullable(true));
			Property(x => x.PollAccess, map => map.NotNullable(true));
			Property(x => x.VoteAccess, map => map.NotNullable(true));
			Property(x => x.ModeratorAccess, map => map.NotNullable(true));
			Property(x => x.EditAccess, map => map.NotNullable(true));
			Property(x => x.DeleteAccess, map => map.NotNullable(true));
			Property(x => x.UploadAccess, map => map.NotNullable(true));
			Property(x => x.DownloadAccess, map => map.NotNullable(true));
			Property(x => x.LastActive);
			Property(x => x.IsGuestX, map => map.NotNullable(true));
        }
    }
}
