using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Project : IEquatable<Project>
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual string ExternalId { get; set; }

        public virtual string OriginalUrl { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual Snapshot LatestPublishedSnapshot { get; set; }

        public virtual int? ForumId { get; set; }

        public virtual ProjectTypeEnum ProjectType { get; set; }

        public virtual bool IsRemoved { get; set; }

        public virtual TextTypeEnum TextType { get; set; }

        public virtual ProjectGroup ProjectGroup { get; set; }

        public virtual IList<Resource> Resources { get; set; }

        public virtual IList<ProjectOriginalAuthor> Authors { get; set; }

        public virtual IList<ProjectResponsiblePerson> ResponsiblePersons { get; set; }

        public virtual IList<LiteraryKind> LiteraryKinds { get; set; }

        public virtual IList<LiteraryGenre> LiteraryGenres { get; set; }

        public virtual IList<LiteraryOriginal> LiteraryOriginals { get; set; }

        public virtual IList<Category> Categories { get; set; }

        public virtual IList<Keyword> Keywords { get; set; }

        public virtual IList<Snapshot> Snapshots { get; set; }

        public virtual IList<Permission> Permissions { get; set; }


        public virtual bool Equals(Project other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Project) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}