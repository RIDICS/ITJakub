using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class CategoryMap : ClassMapping<Category> {
        
        public CategoryMap() {
			Table("yaf_Category");
			Schema("dbo");
			Lazy(false);
			Id(x => x.CategoryID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => { map.NotNullable(true); map.Unique(true); });
			Property(x => x.CategoryImage);
			Property(x => x.SortOrder, map => map.NotNullable(true));
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

			ManyToOne(x => x.PollGroupCluster, map => 
			{
				map.Column("PollGroupID");
				map.NotNullable(false);
				map.Cascade(Cascade.None);
			});

			Bag(x => x.Forums, colmap =>  { colmap.Key(x => x.Column("CategoryID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
