using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.ProjectMetadata;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectInfoManager
    {
        private readonly ProjectRepository m_projectRepository;

        public ProjectInfoManager(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }

        public void SetLiteraryKinds(long projectId, IntegerIdListContract kindIdList)
        {
            new SetLiteraryKindWork(m_projectRepository, projectId, kindIdList.IdList).Execute();
        }

        public void SetLiteraryGenres(long projectId, IntegerIdListContract genreIdList)
        {
            new SetLiteraryGenreWork(m_projectRepository, projectId, genreIdList.IdList).Execute();
        }

        public void SetLiteraryOriginals(long projectId, IntegerIdListContract litOriginalIdList)
        {
            new SetLiteraryOriginalWork(m_projectRepository, projectId, litOriginalIdList.IdList).Execute();
        }

        public void SetAuthors(long projectId, IntegerIdListContract authorIdList)
        {
            new SetAuthorsWork(m_projectRepository, projectId, authorIdList.IdList).Execute();
        }

        public void SetResponsiblePersons(long projectId, List<ProjectResponsiblePersonIdContract> projectResposibleIdList)
        {
            new SetResponsiblePersonsWork(m_projectRepository, projectId, projectResposibleIdList).Execute();
        }
        
        public void SetKeywords(long projectId, IntegerIdListContract keywordIdList)
        {
            new SetKeywordsWork(m_projectRepository, projectId, keywordIdList.IdList).Execute();
        }

        public void SetCategories(long projectId, IntegerIdListContract categoryIdList)
        {
            new SetCategoriesWork(m_projectRepository, projectId, categoryIdList.IdList).Execute();
        }

        public List<LiteraryKindContract> GetLiteraryKinds(long projectId)
        {
            var dbResult = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectLiteraryKinds(projectId));
            var result = Mapper.Map<List<LiteraryKindContract>>(dbResult);
            return result;
        }

        public List<LiteraryGenreContract> GetLiteraryGenres(long projectId)
        {
            var dbResult = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectLiteraryGenres(projectId));
            var result = Mapper.Map<List<LiteraryGenreContract>>(dbResult);
            return result;
        }

        public List<LiteraryOriginalContract> GetLiteraryOriginals(long projectId)
        {
            var dbResult = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectLiteraryOriginals(projectId));
            var result = Mapper.Map<List<LiteraryOriginalContract>>(dbResult);
            return result;
        }

        public List<KeywordContract> GetKeywords(long projectId)
        {
            var dbProject = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectWithKeywords(projectId));
            var result = Mapper.Map<List<KeywordContract>>(dbProject.Keywords);
            return result;
        }

        public List<CategoryContract> GetCategories(long projectId)
        {
            var dbResult = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectCategories(projectId));
            var result = Mapper.Map<List<CategoryContract>>(dbResult);
            return result;
        }

        public List<OriginalAuthorContract> GetAuthors(long projectId)
        {
            var dbResult = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectOriginalAuthorList(projectId, true));
            var result = Mapper.Map<List<OriginalAuthorContract>>(dbResult);
            return result;
        }

        public List<ProjectResponsiblePersonContract> GetProjectResponsiblePersons(long projectId)
        {
            var dbResult = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectResponsibleList(projectId));
            var result = Mapper.Map<List<ProjectResponsiblePersonContract>>(dbResult);
            return result;
        }
    }
}