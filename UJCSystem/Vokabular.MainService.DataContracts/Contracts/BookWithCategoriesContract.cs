using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class BookWithCategoriesContract
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}