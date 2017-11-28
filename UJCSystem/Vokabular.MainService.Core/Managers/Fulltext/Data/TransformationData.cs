using ITJakub.SearchService.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers.Fulltext.Data
{
    public class TransformationData
    {
        public string Name { get; set; }
        public OutputFormatEnumContract OutputFormat { get; set; }
        public ResourceLevelEnumContract ResourceLevel { get; set; }
    }
}
