using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreateLiteraryKindWork : UnitOfWorkBase<int>
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly string m_name;

        public CreateLiteraryKindWork(CatalogValueRepository catalogValueRepository, string name) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryKind = new LiteraryKind
            {
                Name = m_name
            };
            return (int) m_catalogValueRepository.Create(literaryKind);
        }
    }
}