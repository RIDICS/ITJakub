using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Models.Requests.Favorite
{
    public class GetFavoriteLabeledBookRequest
    {
        public IList<long> BookIds { get; set; }
        public BookTypeEnumContract? BookType { get; set; }
    }

    public class GetFavoriteLabeledCategoryRequest
    {
        public IList<int> CategoryIds { get; set; }
    }
}
