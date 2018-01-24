using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.DataContracts
{
    public class CategoryContentContract
    {
        public IList<CategoryContract> Categories { get; set; }

        public IList<BookContract> Books { get; set; }
    }

    public class CategoryOrBookTypeContract : CategoryContract
    {
        public BookTypeEnumContract BookType { get; set; }
    }
}
