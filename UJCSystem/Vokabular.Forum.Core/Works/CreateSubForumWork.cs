using System;
using System.Linq;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Category = Vokabular.ForumSite.DataEntities.Database.Entities.Category;

namespace Vokabular.ForumSite.Core.Works
{
    class CreateSubForumWork : UnitOfWorkBase<int>
    {
        private const string FirstTopicName = "Základní informace";
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly TopicRepository m_topicRepository;
        private readonly MessageRepository m_messageRepository;
        private readonly UserRepository m_userRepository;
        private readonly ForumAccessRepository m_forumAccessRepository;
        private readonly AccessMaskRepository m_accessMaskRepository;
        private readonly GroupRepository m_groupRepository;
        private readonly CategoryContract m_category;

        public CreateSubForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            MessageRepository messageRepository, UserRepository userRepository, ForumAccessRepository forumAccessRepository,
            AccessMaskRepository accessMaskRepository, GroupRepository groupRepository, CategoryContract category,
            UserDetailContract user) :
            base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_accessMaskRepository = accessMaskRepository;
            m_groupRepository = groupRepository;
            m_userRepository = userRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_category = category;
        }

        protected override int ExecuteWorkImplementation()
        {
            int id = 0;

            foreach (UrlBookTypeEnum bookType in BookTypeHelper.GetBookTypeEnumsWithCategories())
            {
                Category category = m_categoryRepository.GetCategoryByExternalId((short)bookType);

                Forum parentForum = null;
                if (m_category.ParentCategoryId != null)
                {
                    parentForum = m_forumRepository.GetForumByExternalIdAndCategory((int)m_category.ParentCategoryId, category);
                }

                Forum forum = new Forum(m_category.Description, category, (short)ForumTypeEnum.SubCategory)
                {
                    ExternalId = m_category.Id,
                    ParentForum = parentForum
                };
                m_forumRepository.Create(forum);
                SetAdminAccessToForumForAdminGroup(forum);
                id = forum.ForumID;
            }

            return id;
        }

        private void SetAdminAccessToForumForAdminGroup(Forum forum)
        {
            AccessMask accessMask = m_accessMaskRepository.FindById<AccessMask>(1); //TODO
            Group group = m_groupRepository.FindById<Group>(1); //TODO
            m_forumAccessRepository.Create(new ForumAccess
            {
                Group = group,
                AccessMask = accessMask,
                Forum = forum
            });
        }
    }
}