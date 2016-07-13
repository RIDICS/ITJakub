using System;
using ITJakub.Web.DataEntities.Database.Entities.Enums;

namespace ITJakub.Web.DataEntities.Database.Entities
{
    public class StaticText
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Text { get; set; }

        public virtual StaticTextFormat Format { get; set; }

        public virtual DateTime ModificationTime { get; set; }

        public virtual string ModificationUser { get; set; }
    }
}
