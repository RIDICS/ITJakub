using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class BbcodeMap : ClassMapping<BBCode> {
        
        public BbcodeMap() {
			Table("yaf_BBCode");
			Schema("dbo");
			Lazy(false);
			Id(x => x.BBCodeID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => map.NotNullable(true));
			Property(x => x.Description);
			Property(x => x.OnClickJS);
			Property(x => x.DisplayJS);
			Property(x => x.EditJS);
			Property(x => x.DisplayCSS);
			Property(x => x.SearchRegex);
			Property(x => x.ReplaceRegex);
			Property(x => x.Variables);
			Property(x => x.UseModule);
			Property(x => x.ModuleClass);
			Property(x => x.ExecOrder, map => map.NotNullable(true));
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

        }
    }
}
