using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.ProjectMetadata;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectMetadataManager
    {
        private readonly MetadataRepository m_metadataRepository;

        public ProjectMetadataManager(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        public int CreatePublisher(PublisherContract data)
        {
            return new CreatePublisherWork(m_metadataRepository, data).Execute();
        }

        public int CreateLiteraryKind(string name)
        {
            return new CreateLiteraryKindWork(m_metadataRepository, name).Execute();
        }

        public int CreateLiteraryGenre(string name)
        {
            return new CreateLiteraryGenreWork(m_metadataRepository, name).Execute();
        }

        public List<PublisherContract> GetPublisherList()
        {
            var result = new GetPublisherListWork(m_metadataRepository).Execute();
            return Mapper.Map<List<PublisherContract>>(result);
        }

        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = new GetLiteraryKindListWork(m_metadataRepository).Execute();
            return Mapper.Map<List<LiteraryKindContract>>(result);
        }

        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            var result = new GetLiteraryGenreListWork(m_metadataRepository).Execute();
            return Mapper.Map<List<LiteraryGenreContract>>(result);
        }

        public ProjectMetadataResultContract GetProjectMetadata(long projectId)
        {
            throw new System.NotImplementedException();
        }

        public long CreateNewProjectMetadataVersion()
        {
            throw new System.NotImplementedException();
        }
    }
}
