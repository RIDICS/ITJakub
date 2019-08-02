using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities
{
    public class GroupMap : ClassMapping<Group>
    {
        public GroupMap()
        {
            Table("yaf_Group");
            Schema("dbo");
            Lazy(false);
            Id(x => x.GroupID, map => map.Generator(Generators.Identity));
            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });
            Property(x => x.Flags, map => map.NotNullable(true));
            Property(x => x.PMLimit, map => map.NotNullable(true));
            Property(x => x.Style);
            Property(x => x.SortOrder, map => map.NotNullable(true));
            Property(x => x.Description);
            Property(x => x.UsrSigChars, map => map.NotNullable(true));
            Property(x => x.UsrSigBBCodes);
            Property(x => x.UsrSigHTMLTags);
            Property(x => x.UsrAlbums, map => map.NotNullable(true));
            Property(x => x.UsrAlbumImages, map => map.NotNullable(true));
            Property(x => x.IsHidden);
            Property(x => x.IsUserGroup);
            ManyToOne(x => x.Board, map =>
            {
                map.Column("BoardID");
                map.Cascade(Cascade.None);
            });

            Bag(x => x.EventLogGroupAccesses, colmap =>
            {
                colmap.Key(x => x.Column("GroupID"));
                colmap.Inverse(true);
            }, map => { map.OneToMany(); });
            Bag(x => x.ForumAccesses, colmap =>
            {
                colmap.Key(x => x.Column("GroupID"));
                colmap.Inverse(true);
            }, map => { map.OneToMany(); });
            Bag(x => x.GroupMedals, colmap =>
            {
                colmap.Key(x => x.Column("GroupID"));
                colmap.Inverse(true);
            }, map => { map.OneToMany(); });
            Bag(x => x.Users, colmap =>
            {
                colmap.Table("yaf_UserGroup");
                colmap.Cascade(Cascade.None);
                colmap.Key(x => x.Column("GroupID"));
            }, map => { map.ManyToMany(p => p.Column("UserID")); });
        }
    }
}