using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ProvMembershipMap : ClassMapping<ProvMembership> {
        
        public ProvMembershipMap() {
			Table("yaf_prov_Membership");
			Schema("dbo");
			Lazy(false);
			Id(x => x.UserID, map => map.Generator(Generators.Assigned));
			Property(x => x.ApplicationID, map => map.NotNullable(true));
			Property(x => x.Username, map => map.NotNullable(true));
			Property(x => x.UsernameLwd, map => map.NotNullable(true));
			Property(x => x.Password);
			Property(x => x.PasswordSalt);
			Property(x => x.PasswordFormat);
			Property(x => x.Email);
			Property(x => x.EmailLwd);
			Property(x => x.PasswordQuestion);
			Property(x => x.PasswordAnswer);
			Property(x => x.IsApproved);
			Property(x => x.IsLockedOut);
			Property(x => x.LastLogin);
			Property(x => x.LastActivity);
			Property(x => x.LastPasswordChange);
			Property(x => x.LastLockOut);
			Property(x => x.FailedPasswordAttempts);
			Property(x => x.FailedAnswerAttempts);
			Property(x => x.FailedPasswordWindow);
			Property(x => x.FailedAnswerWindow);
			Property(x => x.Joined);
			Property(x => x.Comment);
        }
    }
}
