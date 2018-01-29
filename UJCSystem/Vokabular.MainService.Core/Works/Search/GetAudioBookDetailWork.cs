using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class GetAudioBookDetailWork : UnitOfWorkBase<MetadataResource>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly BookRepository m_bookRepository;
        private readonly long m_projectId;

        public GetAudioBookDetailWork(MetadataRepository metadataRepository, BookRepository bookRepository, long projectId) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_bookRepository = bookRepository;
            m_projectId = projectId;
        }

        protected override MetadataResource ExecuteWorkImplementation()
        {
            var metadata = m_metadataRepository.GetMetadataWithFetchForBiblModuleByProject(new[] {m_projectId}).First();

            Tracks = m_bookRepository.GetTracks(m_projectId);
            Recordings = m_bookRepository.GetRecordings(m_projectId);

            return metadata;
        }

        public IList<AudioResource> Recordings { get; set; }

        public IList<TrackResource> Tracks { get; set; }
    }
}
