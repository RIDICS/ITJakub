using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class TopicRepository : NHibernateDao
    {
        public TopicRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual Topic GetFirstTopicInForum(Forum forum)
        {
            return GetSession().QueryOver<Topic>()
                .Where(x => x.Forum == forum)
                .OrderBy(x => x.Posted).Asc
                .Take(1).SingleOrDefault();
        }
    }
}

