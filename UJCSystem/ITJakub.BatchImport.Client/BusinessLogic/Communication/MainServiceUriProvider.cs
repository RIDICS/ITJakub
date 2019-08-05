using System;
using System.Configuration;
using Vokabular.MainService.DataContracts;

namespace ITJakub.BatchImport.Client.BusinessLogic.Communication
{
    public class MainServiceUriProvider : IMainServiceUriProvider
    {
        private const string NewMainServiceEndpointName = "MainService";

        public MainServiceUriProvider()
        {
            var endpointAddress = ConfigurationManager.AppSettings[NewMainServiceEndpointName];
            MainServiceUri = new Uri(endpointAddress);
        }
        
        public Uri MainServiceUri { get; set; }
    }
}
