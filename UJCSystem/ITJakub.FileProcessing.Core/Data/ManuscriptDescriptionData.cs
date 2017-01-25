using System;

namespace ITJakub.FileProcessing.Core.Data
{
    public class ManuscriptDescriptionData
    {
        public string Country { get; set; }
        public string Idno { get; set; }
        public string Title { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        public string OriginDate { get; set; }
        public string Settlement { get; set; }
        public string Repository { get; set; }
    }
}