using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectWorkCategorizationViewModel
    {
        public List<CategoryContract> AllCategoryList { get; set; }
        public List<LiteraryKindContract> AllLiteraryKindList { get; set; }
        public List<LiteraryGenreContract> AllLiteraryGenreList { get; set; }
        
        public HashSet<int> SelectedLiteraryKindIds { get; set; }
        public HashSet<int> SelectedLiteraryGenreIds { get; set; }       
        public HashSet<int> SelectedCategoryIds { get; set; }
        public IList<KeywordContract> SelectedKeywords { get; set; }
    }
}
