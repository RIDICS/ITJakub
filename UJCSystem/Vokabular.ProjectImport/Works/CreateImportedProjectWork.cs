using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectParsing.Model.Entities;
using Project = Vokabular.DataEntities.Database.Entities.Project;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportedProjectWork : UnitOfWorkBase<long>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ImportedRecord m_metadata;
        private readonly int m_userId;

        public CreateImportedProjectWork(ProjectRepository projectRepository, ImportedRecord metadata, int userId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadata = metadata;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);

            var project = new Project
            {
                Name = m_metadata.Project.Name,
                CreateTime = now,
                CreatedByUser = user
            };

            return (long) m_projectRepository.Create(project);
        }
    }
}
