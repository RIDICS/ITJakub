using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class NamedResourceGroupContract
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual TextTypeEnumContract TextType { get; set; }
    }
}