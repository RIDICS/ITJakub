using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class SaveCategorizationRequest
    {
        public List<int> LiteraryKindIdList { get; set; }
        public List<int> LiteraryGenreIdList { get; set; }
        public List<int> KeywordIdList { get; set; }
        public List<int> CategoryIdList { get; set; }
    }
}
