using System.Collections.Generic;
using ITJakub.Contracts.Categories;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class CategoriesViewModel
    {
        public List<SelectionBase> Children { get; set; }
        public string CategoryId { get; set; }
    }
}