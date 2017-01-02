using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works
{
    public class CreateProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ProjectContract m_newData;
        private readonly UserManager m_userManager;
        private long m_resultId;

        public CreateProjectWork(IUnitOfWork unitOfWork, ProjectRepository projectRepository, ProjectContract newData, UserManager userManager) : base(unitOfWork)
        {
            m_projectRepository = projectRepository;
            m_newData = newData;
            m_userManager = userManager;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var currentUser = m_userManager.GetCurrentUser();

            var project = new Project
            {
                Name = m_newData.Name,
                CreateTime = now,
                CreatedByUser = currentUser
            };

            m_resultId = (long) m_projectRepository.Create(project);
        }

        public long GetResultId()
        {
            return m_resultId;
        }
    }
}
