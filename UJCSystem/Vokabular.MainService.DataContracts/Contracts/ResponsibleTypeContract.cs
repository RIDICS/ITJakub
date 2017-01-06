using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResponsibleTypeContract
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ResponsibleTypeEnumContract Type { get; set; }
    }
}