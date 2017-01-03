using ITJakub.Web.DataEntities.Database.Repositories;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub.Installers
{
    public class RepositoryInstaller : IContainerInstaller
    {
        public void Install(IContainer container)
        {
            container.AddPerWebRequest<StaticTextRepository>();
        }
    }
}
