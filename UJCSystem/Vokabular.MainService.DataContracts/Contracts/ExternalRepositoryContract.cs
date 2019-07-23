using System;
using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ExternalRepositoryContract
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string UrlTemplate { get; set; }

        public string License { get; set; }

        public string Configuration { get; set; }
        
        public BibliographicFormatContract BibliographicFormat { get; set; }

        public ExternalRepositoryTypeContract ExternalRepositoryType { get; set; }
    }

    public class ExternalRepositoryDetailContract : ExternalRepositoryContract
    {
        public IList<FilteringExpressionSetContract> FilteringExpressionSets { get; set; }
    }

    public class ExternalRepositoryStatisticsContract
    {
        public int TotalImportedItems { get; set; }

        public int TotalItemsInLastUpdate { get; set; }

        public int NewItemsInLastUpdate { get; set; }

        public int UpdatedItemsInLastUpdate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public UserContract UpdatedBy { get; set; }
        
        public bool IsSuccessful { get; set; }
    }
}