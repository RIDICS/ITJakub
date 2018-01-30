using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class TermCategoryContract
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TermCategoryDetailContract : TermCategoryContract
    {
        public List<TermContract> Terms { get; set; }
    }
}