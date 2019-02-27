using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.CommunicationService
{
    public static class CommunicationServiceContainerRegistration
    {
        public static void AddCommunicationServices(this IServiceCollection services)
        {
           services.AddSingleton<CommunicationProvider>(); 
        }
    }
}
