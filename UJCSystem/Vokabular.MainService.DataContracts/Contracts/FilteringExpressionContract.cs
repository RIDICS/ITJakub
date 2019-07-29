namespace Vokabular.MainService.DataContracts.Contracts
{
    public class FilteringExpressionContract
    {
        public int Id { get; set; }

        public string Field { get; set; }

        public string Value { get; set; }
    }
}