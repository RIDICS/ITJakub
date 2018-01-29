using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Core.Data
{
    public class BookAccessoryData
    {
        public long Id { get; set; }
        public AccessoryType Type { get; set; }
        public string FileName { get; set; }
    }
}