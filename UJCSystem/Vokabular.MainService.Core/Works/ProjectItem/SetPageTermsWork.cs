using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectItem
{
    public class SetPageTermsWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_pageId;
        private readonly IList<int> m_termIdList;

        public SetPageTermsWork(ResourceRepository resourceRepository, long pageId, IList<int> termIdList) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_pageId = pageId;
            m_termIdList = termIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var termList = new List<Term>();
            foreach (var id in m_termIdList)
            {
                var termEntity = m_resourceRepository.Load<Term>(id);
                termList.Add(termEntity);
            }

            var pageResource = m_resourceRepository.GetLatestResourceVersion<PageResource>(m_pageId);
            pageResource.Terms = termList;

            m_resourceRepository.Update(pageResource);
        }
    }
}