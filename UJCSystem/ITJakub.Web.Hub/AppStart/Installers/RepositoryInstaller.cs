using ITJakub.Web.DataEntities.Database.Repositories;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub.Installers
{
    public class RepositoryInstaller : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<StaticTextRepository>();
        }
    }
}
