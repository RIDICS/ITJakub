using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities
{
    public class TermCategory
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual IList<Term> Terms { get; set; }
    }
}