using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works
{
    public class CreateProjectWork : UnitOfWorkBase<long>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ProjectContract m_newData;
        private readonly long m_userId;

        public CreateProjectWork(ProjectRepository projectRepository, ProjectContract newData, long userId) : base(projectRepository.UnitOfWork)
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

            return (long) m_projectRepository.Create(project);
        }
    }
}
