using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class GroupRepository : NHibernateDao
    {
        public GroupRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual Group GetGroupByName(string name)
        {
            return GetSession().QueryOver<Group>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }
    }
}