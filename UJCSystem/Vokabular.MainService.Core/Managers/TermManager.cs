using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.CatalogValues;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class TermManager
    {
        private readonly CatalogValueRepository m_catalogValueRepository;

        public TermManager(CatalogValueRepository catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
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
            return Mapper.Map<TermContract>(term);
        }

        public List<TermContract> GetTermList(int? categoryId)
        {
            var resultList = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetTermList(categoryId));
            return Mapper.Map<List<TermContract>>(resultList);
        }
    }
}
