using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
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

        public int CreateLiteraryKind(string name)
        {
            return new CreateLiteraryKindWork(m_metadataRepository, name).Execute();
        }

        public int CreateLiteraryGenre(string name)
        {
            return new CreateLiteraryGenreWork(m_metadataRepository, name).Execute();
        }

        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetLiteraryKindList());
            return Mapper.Map<List<LiteraryKindContract>>(result);
        }

        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetLiteraryGenreList());
            return Mapper.Map<List<LiteraryGenreContract>>(result);
        }

        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetLiteraryOriginalList());
            return Mapper.Map<List<LiteraryOriginalContract>>(result);
        }

        public ProjectMetadataResultContract GetProjectMetadata(long projectId, GetProjectMetadataParameter parameters)
        {
            var work = new GetLatestProjectMetadataWork(m_metadataRepository, projectId, parameters);
            var result = work.Execute();
            var resultContract = result != null
                ? Mapper.Map<ProjectMetadataResultContract>(result)
                : new ProjectMetadataResultContract();

            if (result != null && parameters.IsAnyAdditionalParameter())
            {
                var project = result.Resource.Project;

                if (parameters.IncludeAuthor)
                {
                    resultContract.AuthorList = Mapper.Map<List<OriginalAuthorContract>>(project.Authors);
                }
                if (parameters.IncludeResponsiblePerson)
                {
                    resultContract.ResponsiblePersonList = Mapper.Map<List<ProjectResponsiblePersonContract>>(project.ResponsiblePersons);
                }
                if (parameters.IncludeKind)
                {
                    resultContract.LiteraryKindList = Mapper.Map<List<LiteraryKindContract>>(project.LiteraryKinds);
                }
                if (parameters.IncludeGenre)
                {
                    resultContract.LiteraryGenreList = Mapper.Map<List<LiteraryGenreContract>>(project.LiteraryGenres);
                }
                if (parameters.IncludeOriginal)
                {
                    resultContract.LiteraryOriginalList = Mapper.Map<List<LiteraryOriginalContract>>(project.LiteraryOriginals);
                }
            }

            return resultContract;
        }

        public long CreateNewProjectMetadataVersion(long projectId, ProjectMetadataContract data)
        {
            var resultId = new CreateNewMetadataVersionWork(m_metadataRepository, projectId, data, m_userManager.GetCurrentUserId()).Execute();
            return resultId;
        }

        public void SetLiteraryKinds(long projectId, IntegerIdListContract kindIdList)
        {
            new SetLiteraryKindWork(m_metadataRepository, projectId, kindIdList.IdList).Execute();
        }

        public void SetLiteraryGenres(long projectId, IntegerIdListContract genreIdList)
        {
            new SetLiteraryGenreWork(m_metadataRepository, projectId, genreIdList.IdList).Execute();
        }

        public void SetAuthors(long projectId, IntegerIdListContract authorIdList)
        {
            new SetAuthorsWork(m_metadataRepository, projectId, authorIdList.IdList).Execute();
        }

        public void SetResponsiblePersons(long projectId, List<ProjectResponsiblePersonIdContract> projectResposibleIdList)
        {
            new SetResponsiblePersonsWork(m_metadataRepository, projectId, projectResposibleIdList).Execute();
        }
    }
}
