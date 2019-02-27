using System;

namespace Vokabular.ProjectImport.DataEntities.Database
{
    public class Resource
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string License { get; set; }
        public ResourceType ResourceType { get; set; }
        public ParserType ParserType { get; set; }
        public DateTime LastUpdate { get; set; }
        public int NewItemsCount { get; set; }
        public int UpdatedItemsCount { get; set; }
        public int TotalItemsCount { get; set; }
        public string LastUpdateBy { get; set; } //TODO change string to User
        public string Configuration { get; set; } //TODO to JSON object
    }
}
