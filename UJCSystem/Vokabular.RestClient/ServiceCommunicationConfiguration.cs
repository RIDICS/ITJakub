using System;

namespace Vokabular.RestClient
{
    public class ServiceCommunicationConfiguration
    {
        public Uri Url { get; set; }

        public bool CreateCustomHandler { get; set; }
    }
}