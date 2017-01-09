using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class GetLiteraryGenreListWork : UnitOfWorkBase<IList<LiteraryGenre>>
    {
        private readonly MetadataRepository m_metadataRepository;

        public GetLiteraryGenreListWork(MetadataRepository metadataRepository) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
        }

        protected override IList<LiteraryGenre> ExecuteWorkImplementation()
        {
            return m_metadataRepository.GetLiteraryGenreList();
        }
    }
}