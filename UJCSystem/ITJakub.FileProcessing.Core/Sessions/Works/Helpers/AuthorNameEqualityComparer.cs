using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Sessions.Works.Helpers
{
    public class AuthorNameEqualityComparer : IEqualityComparer<OriginalAuthor>
    {
        public bool Equals(OriginalAuthor author1, OriginalAuthor author2)
        {
            return string.Equals(author1.FirstName, author2.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(author1.LastName, author2.LastName, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(OriginalAuthor obj)
        {
            return obj.GetHashCode();
        }
    }
}