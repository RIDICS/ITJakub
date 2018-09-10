using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetCategoriesWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_categoryIdList;

        public SetCategoriesWork(ProjectRepository projectRepository, long projectId, IList<int> categoryIdList) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_categoryIdList = categoryIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var categoryList = new List<Category>();
            foreach (var id in m_categoryIdList)
            {
                var category = m_projectRepository.Load<Category>(id);
                categoryList.Add(category);
            }

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.Categories = categoryList;

            m_projectRepository.Update(project);
        }
    }
}