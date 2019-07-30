using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class MailMap : ClassMapping<Mail> {
        
        public MailMap() {
			Table("yaf_Mail");
			Schema("dbo");
			Lazy(false);
			Id(x => x.MailID, map => map.Generator(Generators.Identity));
			Property(x => x.FromUser, map => map.NotNullable(true));
			Property(x => x.FromUserName);
			Property(x => x.ToUser, map => map.NotNullable(true));
			Property(x => x.ToUserName);
			Property(x => x.Created, map => map.NotNullable(true));
			Property(x => x.Subject, map => map.NotNullable(true));
			Property(x => x.Body, map => map.NotNullable(true));
			Property(x => x.BodyHtml);
			Property(x => x.SendTries, map => map.NotNullable(true));
			Property(x => x.SendAttempt);
			Property(x => x.ProcessID);
        }
    }
}
