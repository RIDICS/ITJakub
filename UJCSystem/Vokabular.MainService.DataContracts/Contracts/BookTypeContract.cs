using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class BookTypeContract
    {
        public int Id { get; set; }

        public BookTypeEnumContract Type { get; set; }
    }
}