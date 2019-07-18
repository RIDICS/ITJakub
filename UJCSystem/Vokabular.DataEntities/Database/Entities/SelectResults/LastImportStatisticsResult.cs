namespace Vokabular.DataEntities.Database.Entities.SelectResults
{
    public class LastImportStatisticsResult
    {
        public int TotalItems { get; set; }

        public int NewItems { get; set; }
        
        public int UpdatedItems { get; set; }

        public int FailedItems { get; set; }
        
        public User UpdatedByUser { get; set; }
    }
}