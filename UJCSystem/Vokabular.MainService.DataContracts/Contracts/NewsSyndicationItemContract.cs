using System;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class NewsSyndicationItemContract
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public NewsTypeEnumContract ItemType { get; set; }

        public DateTime CreateTime { get; set; }
        
        public UserContract CreatedByUser { get; set; }
    }

    public class CreateNewsSyndicationItemContract
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public NewsTypeEnumContract ItemType { get; set; }
    }
}
