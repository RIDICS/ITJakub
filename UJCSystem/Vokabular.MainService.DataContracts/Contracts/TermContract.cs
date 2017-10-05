namespace Vokabular.MainService.DataContracts.Contracts
{
    public class TermContract
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Position { get; set; }
        public int CategoryId { get; set; }
    }
}