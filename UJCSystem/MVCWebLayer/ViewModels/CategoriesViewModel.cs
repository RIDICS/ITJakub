using ITJakub.Contracts.Categories;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class CategoriesViewModel
    {
        public SelectionBase[] Children { get; set; }
        public string CategoryId { get; set; }
    }
}