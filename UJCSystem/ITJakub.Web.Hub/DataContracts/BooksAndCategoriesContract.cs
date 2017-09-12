using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.DataContracts
{
    public class BooksAndCategoriesContract
    {
        public BookTypeEnumContract BookType { get; set; }

        public IList<CategoryContract> Categories { get; set; }

        public IList<BookWithCategoryIdsContract> Books { get; set; }
    }

    public class BookWithCategoryIdsContract
    {
        public long Id { get; set; }

        public string Guid { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public IList<int> CategoryIds { get; set; }
    }
}
