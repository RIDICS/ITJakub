using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class UserprofileMap : ClassMapping<UserProfile> {
        
        public UserprofileMap() {
			Table("yaf_UserProfile");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.UserID, m => m.Column("UserID"));
					compId.Property(x => x.ApplicationName, m => m.Column("ApplicationName"));
				});
			Property(x => x.LastUpdatedDate, map => map.NotNullable(true));
			Property(x => x.LastActivity);
			Property(x => x.IsAnonymous, map => map.NotNullable(true));
			Property(x => x.UserName, map => map.NotNullable(true));
			Property(x => x.Gender);
			Property(x => x.Blog);
			Property(x => x.RealName);
			Property(x => x.Interests);
			Property(x => x.Skype);
			Property(x => x.Facebook);
			Property(x => x.Location);
			Property(x => x.BlogServiceUrl);
			Property(x => x.Birthday);
			Property(x => x.LastSyncedWithDNN);
			Property(x => x.ICQ);
			Property(x => x.City);
			Property(x => x.MSN);
			Property(x => x.TwitterId);
			Property(x => x.Twitter);
			Property(x => x.BlogServicePassword);
			Property(x => x.Country);
			Property(x => x.Occupation);
			Property(x => x.Region);
			Property(x => x.AIM);
			Property(x => x.XMPP);
			Property(x => x.YIM);
			Property(x => x.Google);
			Property(x => x.BlogServiceUsername);
			Property(x => x.GoogleId);
			Property(x => x.Homepage);
			Property(x => x.FacebookId);
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
