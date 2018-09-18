using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Managers
{
    public class ForumManager
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;

        public ForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
        }

        public Forum GetForum(int forumId)
        {
            return m_forumRepository.InvokeUnitOfWork(x => x.FindById<Forum>(forumId));
        }

        public long CreateNewForum(Project project, IList<BookType> bookTypes, UserDetailContract user)
        {
            var work = new CreateForumWork(m_forumRepository, m_categoryRepository, project, bookTypes, user);
            var resultId = work.Execute();
            return resultId;
        }
    }
}
