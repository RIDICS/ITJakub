using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class GetForumWork : UnitOfWorkBase<Forum>
    {
        private readonly ForumRepository m_forumRepository;
        private readonly long m_projectId;

        public GetForumWork(ForumRepository forumRepository, long projectId) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_projectId = projectId;
        }

        protected override Forum ExecuteWorkImplementation()
        {
            return m_forumRepository.GetMainForumByExternalProjectId(m_projectId);
        }
    }
}
