using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ProvProfileMap : ClassMapping<ProvProfile> {
        
        public ProvProfileMap() {
			Table("yaf_prov_Profile");
			Schema("dbo");
			Lazy(false);
			Id(x => x.UserID, map => map.Generator(Generators.Assigned));
			Property(x => x.LastUpdatedDate, map => map.NotNullable(true));
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
        }
    }
}
