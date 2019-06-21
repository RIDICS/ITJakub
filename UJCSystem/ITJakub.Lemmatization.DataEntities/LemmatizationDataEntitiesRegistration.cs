using ITJakub.Lemmatization.DataEntities.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.DataEntities
{
    public class LemmatizationDataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.TryAddScoped<UnitOfWorkProvider>();

            services.AddScoped<LemmatizationRepository>();
        }
    }
}
