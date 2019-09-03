using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.ExternalBibliography
{
    public class FilteringExpressionSetContract
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public BibliographicFormatContract BibliographicFormat { get; set; }
    }

    public class FilteringExpressionSetDetailContract : FilteringExpressionSetContract
    {
        public UserContract CreatedByUser { get; set; }

        public IList<FilteringExpressionContract> FilteringExpressions { get; set; }
    }
    
}