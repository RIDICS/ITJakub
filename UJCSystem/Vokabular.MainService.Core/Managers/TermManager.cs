using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.CatalogValues;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class TermManager
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly IMapper m_mapper;

        public TermManager(CatalogValueRepository catalogValueRepository, IMapper mapper)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_mapper = mapper;
        }

        public int CreateTerm(TermContract data)
        {
            return new CreateOrUpdateTermWork(m_catalogValueRepository, null, data).Execute();
        }

        public void UpdateTerm(int termId, TermContract data)
        {
            new CreateOrUpdateTermWork(m_catalogValueRepository, termId, data).Execute();
        }

        public void DeleteTerm(int termId)
        {
            new DeleteTermWork(m_catalogValueRepository, termId).Execute();
        }

        public TermContract GetTerm(int termId)
        {
            var term = m_catalogValueRepository.InvokeUnitOfWork(x => x.FindById<Term>(termId));
            return m_mapper.Map<TermContract>(term);
        }

        public List<TermContract> GetTermList(int? categoryId)
        {
            var resultList = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetTermList(categoryId));
            return m_mapper.Map<List<TermContract>>(resultList);
        }
    }
}
