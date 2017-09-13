using System.Collections.Generic;
using ITJakub.Shared.Contracts.Favorites;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Models.Requests.Favorite
{
    public class CreateFavoriteQueryRequest
    {
        public BookTypeEnumContract BookType { get; set; }
        public QueryTypeEnumContract QueryType { get; set; }
        public string Query { get; set; }
        public string Title { get; set; }
        public IList<long> LabelIds { get; set; }
    }
}