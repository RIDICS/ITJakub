using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
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
        private readonly Project m_project;
        private readonly IList<BookType> m_bookTypes;
        private readonly UserDetailContract m_user;

        public CreateForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, Project project,
            IList<BookType> bookTypes, UserDetailContract user) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_project = project;
            m_bookTypes = bookTypes;
            m_user = user;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;

            Category category = m_categoryRepository.CreateOrGetCategoryByName(m_bookTypes.First().Type.ToString());

            Forum forum = new Forum(m_project.Name, category, 10);
            long id = (long) m_forumRepository.Create(forum);
            //TODO create forum access

            for (int i = 1; i < m_bookTypes.Count; i++)
            {
                Forum tempForum = new Forum(m_project.Name, m_categoryRepository.CreateOrGetCategoryByName(m_bookTypes[i].Type.ToString()),
                    (short) ForumTypeEnum.Forum);
                m_forumRepository.Create(tempForum);
                //TODO create forum access
            }

            //TODO create first topic

            return id;
        }
    }
}