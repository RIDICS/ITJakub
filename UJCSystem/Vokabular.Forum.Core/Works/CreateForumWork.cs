using System;
using System.Collections.Generic;
using System.Text;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;
using User = Vokabular.DataEntities.Database.Entities.User;

namespace Vokabular.ForumSite.Core.Works
{
    class CreateForumWork : UnitOfWorkBase<long>
    {
        private readonly ForumRepository m_forumRepository;
        private readonly int m_userId;


        public CreateForumWork(ForumRepository forumRepository, int userId) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var currentUser = m_forumRepository.Load<User>(m_userId);

            Forum forum = new Forum();

            return (long)m_forumRepository.Create(forum);
        }
    }

   /* public class CreateProjectWork : UnitOfWorkBase<long>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ProjectContract m_newData;
        private readonly int m_userId;

        public CreateProjectWork(ProjectRepository projectRepository, ProjectContract newData, int userId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_newData = newData;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var currentUser = m_projectRepository.Load<User>(m_userId);

            var project = new Project
            {
                Name = m_newData.Name,
                CreateTime = now,
                CreatedByUser = currentUser
            };

            return (long)m_projectRepository.Create(project);
        }
    }*/
}
