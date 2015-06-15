namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SourcesContentViewModel
    {
        private const int CharCountToPage = 1800;

        public string Id { get; set; }
        public string Content { get; set; }
        public int Page { get; set; }
        public int PageCount { get { return Content.Length/CharCountToPage; } }



        public int GetNextPage()
        {
            if (Page >= PageCount)
            {
                return PageCount;
            }
            return Page + 1;
        }

        public int GetPrevPage()
        {
            if (Page < 2)
                return 1;
            return Page - 1;
        }

        public bool First
        {
            get { return Page == 1; }
        }


        public int FirstPage
        {
            get { return 1; }
        }

        public bool Last
        {
            get { return Page == PageCount; }
        }

        public int LastPage
        {
            get { return PageCount; }
        }

        public int CurrentPage
        {
            get { return Page; }
        }
    }
}