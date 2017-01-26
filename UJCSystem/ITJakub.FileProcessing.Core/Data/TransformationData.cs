using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Core.Data
{
    public class TransformationData
    {
        public bool IsDefaultForBookType { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public OutputFormatEnum OutputFormat { get; set; }
        public ResourceLevelEnum ResourceLevel { get; set; }
        public BookTypeEnum BookType { get; set; }
    }
}