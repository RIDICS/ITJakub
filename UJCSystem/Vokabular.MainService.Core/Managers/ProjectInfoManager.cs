using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
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
    }
}