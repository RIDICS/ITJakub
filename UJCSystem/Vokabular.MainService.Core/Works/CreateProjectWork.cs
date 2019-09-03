using System;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class CreateProjectWork : UnitOfWorkBase<long>
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
            var projectType = Mapper.Map<ProjectTypeEnum>(m_newData.ProjectType);

            var project = new Project
            {
                Name = m_newData.Name,
                ProjectType = projectType,
                CreateTime = now,
                CreatedByUser = currentUser
            };

            return (long) m_projectRepository.Create(project);
        }
    }
}
