using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class ResponsibleData
    {
        public string NameText { get; set; }
        public string TypeText { get; set; }
        public ResponsibleTypeEnum TypeEnum { get; set; }
    }
}
