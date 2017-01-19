using ITJakub.Web.DataEntities.Database.Repositories;
using ITJakub.Web.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Container;

namespace ITJakub.Web.DataEntities
{
    public class WebDataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<IUnitOfWork, UnitOfWork>();

            container.AddPerWebRequest<StaticTextRepository>();
        }
    }
}
