using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class PageResultContextContract
    {
        public long PageId { get; set; }

        public string PageName { get; set; }

        public KwicStructure ContextStructure { get; set; }
    }
}
