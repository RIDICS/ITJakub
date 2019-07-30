using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ChoiceMap : ClassMapping<Choice> {
        
        public ChoiceMap() {
			Table("yaf_Choice");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ChoiceID, map => map.Generator(Generators.Identity));
			Property(x => x.ChoiceText, map => map.NotNullable(true));
			Property(x => x.Votes, map => map.NotNullable(true));
			Property(x => x.ObjectPath);
			Property(x => x.MimeType);
			ManyToOne(x => x.Poll, map => 
			{
				map.Column("PollID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
