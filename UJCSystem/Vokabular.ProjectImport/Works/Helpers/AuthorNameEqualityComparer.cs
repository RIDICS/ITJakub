using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;

namespace Vokabular.ProjectImport.Works.Helpers
{
    public class AuthorNameEqualityComparer : IEqualityComparer<OriginalAuthor>
    {
        public bool Equals(OriginalAuthor author1, OriginalAuthor author2)
        {
            if (author1 == null || author2 == null)
                return false;

            return string.Equals(author1.FirstName, author2.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(author1.LastName, author2.LastName, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(OriginalAuthor obj)
        {
            return ((obj.FirstName != null ? obj.FirstName.GetHashCode() : 0) * 397) ^ (obj.LastName != null ? obj.LastName.GetHashCode() : 0);
        }
    }
}