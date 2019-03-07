namespace Vokabular.CommunicationService.OAIPMH
{
    public class OaiPmhResource
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public string DataFormat { get; set; }
        public string SetName { get; set; }

        public int Priority { get; set; }
        public string TemplateUrl { get; set; }
    }
}