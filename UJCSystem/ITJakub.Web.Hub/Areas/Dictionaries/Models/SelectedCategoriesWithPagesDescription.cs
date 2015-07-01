namespace ITJakub.Web.Hub.Areas.Dictionaries.Models
{
    public class SelectedCategoriesWithPagesDescription
    {
        public SelectedCategoriesDescription Categories { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}