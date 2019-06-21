using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ForumSite.DataEntities.Database.Repositories;

namespace Vokabular.MainService
{
    public class IocServiceKeys
    {
        public const string Main = MainDbRepositoryBase.ServiceKey;
        public const string Forum = ForumDbRepositoryBase.ServiceKey;
    }
}