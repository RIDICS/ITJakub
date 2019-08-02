using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;

namespace Vokabular.ForumSite.Core.Works.Subworks
{
    public class ForumAccessSubwork
    {
        private readonly AccessMaskRepository m_accessMaskRepository;
        private readonly GroupRepository m_groupRepository;

        public ForumAccessSubwork(AccessMaskRepository accessMaskRepository, GroupRepository groupRepository)
        {
            m_accessMaskRepository = accessMaskRepository;
            m_groupRepository = groupRepository;
        }

        public virtual void SetAdminAccessToForumForAdminGroup(Forum forum)
        {
            SetAccessToForumForGroup(forum, "Admin Access", "Administrators");
        }

        public virtual void SetMemberAccessToForumForRegisteredGroup(Forum forum)
        {
            SetAccessToForumForGroup(forum, "Member Access", "Registered");
        }

        private void SetAccessToForumForGroup(Forum forum, string accessMaskName, string groupName)
        {
            AccessMask accessMask = m_accessMaskRepository.GetAccessMaskByNameAndBoard(accessMaskName, forum.Category.Board.BoardID);
            Group group = m_groupRepository.GetGroupByNameAndBoard(groupName, forum.Category.Board.BoardID);

            m_accessMaskRepository.Create(new ForumAccess
            {
                Group = group,
                AccessMask = accessMask,
                Forum = forum
            });
        }
    }
}
