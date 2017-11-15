using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.ProjectMetadata;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class CatalogValueManager
    {
        private readonly CatalogValueRepository m_catalogValueRepository;

        public CatalogValueManager(CatalogValueRepository catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
        }

        public int CreateLiteraryKind(string name)
        {
            return new CreateLiteraryKindWork(m_catalogValueRepository, name).Execute();
        }

        public int CreateLiteraryGenre(string name)
        {
            return new CreateLiteraryGenreWork(m_catalogValueRepository, name).Execute();
        }

        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetLiteraryKindList());
            return Mapper.Map<List<LiteraryKindContract>>(result);
        }

        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetLiteraryGenreList());
            return Mapper.Map<List<LiteraryGenreContract>>(result);
        }

        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetLiteraryOriginalList());
            return Mapper.Map<List<LiteraryOriginalContract>>(result);
        }
    }
}
