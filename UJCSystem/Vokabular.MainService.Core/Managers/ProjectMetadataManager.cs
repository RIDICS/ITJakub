using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.Core.Works.ProjectMetadata;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectMetadataManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly UserManager m_userManager;

        public ProjectMetadataManager(MetadataRepository metadataRepository, UserManager userManager)
        {
            m_metadataRepository = metadataRepository;
            m_userManager = userManager;
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

        public ProjectMetadataResultContract GetProjectMetadata(long projectId, GetProjectMetadataParameter parameters)
        {
            var work = new GetLatestProjectMetadataWork(m_metadataRepository, projectId, parameters);
            var result = work.Execute();
            var resultContract = result != null
                ? Mapper.Map<ProjectMetadataResultContract>(result)
                : new ProjectMetadataResultContract {Publisher = new PublisherContract()};

            if (result != null && parameters.IsAnyAdditionalParameter())
            {
                var project = result.Resource.Project;

                if (parameters.IncludeAuthor)
                {
                    resultContract.AuthorList = Mapper.Map<List<OriginalAuthorContract>>(project.Authors);
                }
                if (parameters.IncludeResponsiblePerson)
                {
                    resultContract.ResponsiblePersonList = Mapper.Map<List<ResponsiblePersonContract>>(project.ResponsiblePersons);
                }
                if (parameters.IncludeKind)
                {
                    resultContract.LiteraryKindList = Mapper.Map<List<LiteraryKindContract>>(project.LiteraryKinds);
                }
                if (parameters.IncludeGenre)
                {
                    resultContract.LiteraryGenreList = Mapper.Map<List<LiteraryGenreContract>>(project.LiteraryGenres);
                }
            }

            return resultContract;
        }

        public long CreateNewProjectMetadataVersion(long projectId, ProjectMetadataContract data)
        {
            var resultId = new CreateNewMetadataVersionWork(m_metadataRepository, projectId, data, m_userManager.GetCurrentUserId()).Execute();
            return resultId;
        }
    }
}
