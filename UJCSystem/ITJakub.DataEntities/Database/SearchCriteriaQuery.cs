using System.Collections.Generic;

namespace ITJakub.DataEntities.Database
{
    public class SearchCriteriaQuery
    {
        public string Join { get; set; }
        
        public string Where { get; set; }

        public List<string> Parameters { get; set; }
    }
}