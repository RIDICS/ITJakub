namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    public class TokenDistanceCriteriaDescription
    {
        public int Distance { get; set; }
        public WordCriteriaDescription First { get; set; }
        public WordCriteriaDescription Second { get; set; }
    }
}