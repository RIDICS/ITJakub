using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Category = Vokabular.ForumSite.DataEntities.Database.Entities.Category;

namespace Vokabular.ForumSite.Core.Works
{
    class CreateForumWork : UnitOfWorkBase<long>
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ProjectDetailContract m_project;
        private readonly short[] m_bookTypeIds;
        private readonly UserDetailContract m_user;

        public CreateForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, ProjectDetailContract project,
            short[] bookTypeIds, UserDetailContract user) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_project = project;
            m_bookTypeIds = bookTypeIds;
            m_user = user;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;

            Category category = m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds.First());

            Forum forum = new Forum(m_project.Name, category, 10);
            m_forumRepository.Create(forum);
            //TODO create forum access
           
            for (int i = 1; i < m_bookTypeIds.Length; i++)
            {
                Forum tempForum = new Forum(m_project.Name, m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds[i]),
                    (short) ForumTypeEnum.Forum);
                m_forumRepository.Create(tempForum);
                //TODO create forum access
            }

            //TODO create first topic

            return forum.ForumID;
        }
    }
}