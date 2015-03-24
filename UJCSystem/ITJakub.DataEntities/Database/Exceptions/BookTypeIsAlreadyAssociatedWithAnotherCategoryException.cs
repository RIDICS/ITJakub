using System;

namespace ITJakub.DataEntities.Database.Exceptions
{
    public class BookTypeIsAlreadyAssociatedWithAnotherCategoryException : Exception
    {
        public BookTypeIsAlreadyAssociatedWithAnotherCategoryException(long bookTypeId, long rootCategoryId)
            : base(string.Format("Typ knihy s id '{0}' je již spojen s jinou existující kategorií s id '{1}'", bookTypeId, rootCategoryId))
        {
        }
    }
}