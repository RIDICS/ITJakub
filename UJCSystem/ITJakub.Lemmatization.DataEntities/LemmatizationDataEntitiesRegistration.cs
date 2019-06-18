using ITJakub.Lemmatization.DataEntities.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.DataEntities
{
    public class LemmatizationDataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<LemmatizationRepository>();
        }
    }
}
