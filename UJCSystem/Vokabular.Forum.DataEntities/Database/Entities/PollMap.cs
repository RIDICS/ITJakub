using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class PollMap : ClassMapping<Poll> {
        
        public PollMap() {
			Table("yaf_Poll");
			Schema("dbo");
			Lazy(false);
			Id(x => x.PollID, map => map.Generator(Generators.Identity));
			Property(x => x.Question, map => map.NotNullable(true));
			Property(x => x.Closes);
			Property(x => x.UserID, map => map.NotNullable(true));
			Property(x => x.ObjectPath);
			Property(x => x.MimeType);
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.IsClosedBound);
			Property(x => x.AllowMultipleChoices);
			Property(x => x.ShowVoters);
			Property(x => x.AllowSkipVote);
			ManyToOne(x => x.PollGroupCluster, map => 
			{
				map.Column("PollGroupID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			Bag(x => x.Choices, colmap =>  { colmap.Key(x => x.Column("PollID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.PollVotes, colmap =>  { colmap.Key(x => x.Column("PollID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
