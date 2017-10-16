namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class HitSettingsContract
    {
        public int? Count { get; set; }

        public int? Start { get; set; }

        public int ContextLength { get; set; }
    }
}