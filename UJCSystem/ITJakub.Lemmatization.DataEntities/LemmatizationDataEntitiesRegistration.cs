using ITJakub.Lemmatization.DataEntities.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Container;

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
