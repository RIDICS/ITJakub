using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class FavoritetopicMap : ClassMapping<FavoriteTopic> {
        
        public FavoritetopicMap() {
			Table("yaf_FavoriteTopic");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ID, map => map.Generator(Generators.Identity));
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Topic, map => 
			{
				map.Column("TopicID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
