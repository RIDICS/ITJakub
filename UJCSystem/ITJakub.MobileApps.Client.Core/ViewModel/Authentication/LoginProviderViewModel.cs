using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.Core.ViewModel.Authentication
{
    public class LoginProviderViewModel
    {
        public string Name { get; set; }
        public AuthProvidersContract LoginProviderType { get; set; }
    }
}