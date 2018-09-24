using System.Collections.Generic;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class ForumAccessRepository : NHibernateDao
    {
        private AccessMaskRepository m_accessMaskRepository;
        private GroupRepository m_groupRepository;

        public ForumAccessRepository(IUnitOfWork unitOfWork, AccessMaskRepository accessMaskRepository, GroupRepository groupRepository) :
            base(unitOfWork)
        {
            m_accessMaskRepository = accessMaskRepository;
            m_groupRepository = groupRepository;
        }

        public void SetAdminAccessToForumForAdminGroup(Forum forum)
        {
            AccessMask accessMask = m_accessMaskRepository.FindById<AccessMask>(1); //TODO
            Group group = m_groupRepository.FindById<Group>(1); //TODO
            Create(new ForumAccess
            {
                Group = group,
                AccessMask = accessMask,
                Forum = forum
            });
        }

        public void RemoveAllAccessesFromForum(Forum forum)
        {
            DeleteAll(GetAllAccessesForForum(forum));
        }

        public IList<ForumAccess> GetAllAccessesForForum(Forum forum)
        {
            return GetSession().QueryOver<ForumAccess>()
                .Where(x => x.Forum == forum)
                .List();
        }
    }
}