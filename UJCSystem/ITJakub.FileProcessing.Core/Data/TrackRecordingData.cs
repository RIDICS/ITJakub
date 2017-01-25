using System;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Core.Data
{
    public class TrackRecordingData
    {
        public string FileName { get; set; }
        public AudioTypeEnum AudioType { get; set; }
        public string MimeType { get; set; }
        public TimeSpan Length { get; set; }
    }
}