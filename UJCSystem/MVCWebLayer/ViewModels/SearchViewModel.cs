using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public string SelectedTerm { get; set; }
        
        public string Dila { get; set; }
        public string Kategorie { get; set; }
        public string BookId { get; set; }
    }
}