using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.Lemmatization.DataEntities
{
    public class LemmatizationDataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<IUnitOfWork, UnitOfWork>();

            container.AddPerWebRequest<LemmatizationRepository>();
        }
    }
}
