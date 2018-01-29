using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Core.Data
{
    public class ResponsibleData
    {
        public string NameText { get; set; }
        public string TypeText { get; set; }
        public ResponsibleTypeEnum TypeEnum { get; set; }
    }
}