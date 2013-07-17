
namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SourcesContentViewModel
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        
        public int NextPage()
        {
            if (this.Page >= this.PageCount) {
                return this.PageCount;
            }
            return this.Page + 1;
        }

        public int PrevPage()
        {
            if (this.Page < 2) {
                return 1;
            }
            return this.Page - 1;
        }

        public bool first()
        {
            return this.Page == 1;
        }

        public int FirstPage()
        {
            return 1;
        }

        public bool last()
        {
            return this.Page == this.PageCount;
        }

        public int LastPage()
        {
            return this.PageCount;
        }

        public int CurrentPage()
        {
            return this.Page;
        }
    }
}