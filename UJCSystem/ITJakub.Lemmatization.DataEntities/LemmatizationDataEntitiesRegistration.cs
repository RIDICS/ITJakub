using ITJakub.Lemmatization.DataEntities.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Container;

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
