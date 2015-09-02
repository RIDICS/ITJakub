using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities.SelectResults
{
    public class TermPageResult
    {
        public long BookId { get; set; }

        public IList<BookPage> Pages { get; set; }
    }
}