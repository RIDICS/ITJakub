using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchAudioByCriteriaWork : UnitOfWorkBase<IList<MetadataResource>>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly SearchCriteriaQueryCreator m_queryCreator;

        public SearchAudioByCriteriaWork(MetadataRepository metadataRepository, SearchCriteriaQueryCreator queryCreator) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_queryCreator = queryCreator;
        }

        protected override IList<MetadataResource> ExecuteWorkImplementation()
        {
            var metadataList = m_metadataRepository.SearchByCriteriaQuery(m_queryCreator);

            var metadataIdList = metadataList.Select(x => x.Id);
            m_metadataRepository.GetMetadataWithFetchForBiblModule(metadataIdList);
            
            var projectIdList = metadataList.Select(x => x.Resource.Project.Id);
            var fullBookRecordings = m_metadataRepository.GetFullBookRecordings(projectIdList);
            FullBookRecordingsByProjectId = fullBookRecordings.GroupBy(key => key.Resource.Project.Id).ToDictionary(key => key.Key, val => val.ToList());

            return metadataList;
        }

        public Dictionary<long, List<AudioResource>> FullBookRecordingsByProjectId { get; set; }
    }
}