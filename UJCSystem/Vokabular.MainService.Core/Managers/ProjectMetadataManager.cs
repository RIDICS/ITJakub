using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.ProjectMetadata;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectMetadataManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly UserManager m_userManager;
        private readonly CategoryRepository m_categoryRepository;

        public ProjectMetadataManager(MetadataRepository metadataRepository, UserManager userManager, CategoryRepository categoryRepository)
        {
            m_metadataRepository = metadataRepository;
            m_userManager = userManager;
            m_categoryRepository = categoryRepository;
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

        public List<string> GetPublisherAutocomplete(string query)
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetPublisherAutocomplete(query, DefaultValues.AutocompleteMaxCount));
            return result.ToList();
        }

        public List<string> GetCopyrightAutocomplete(string query)
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetCopyrightAutocomplete(query, DefaultValues.AutocompleteMaxCount));
            return result.ToList();
        }

        public List<string> GetManuscriptRepositoryAutocomplete(string query)
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetManuscriptRepositoryAutocomplete(query, DefaultValues.AutocompleteMaxCount));
            return result.ToList();
        }

        public List<string> GetTitleAutocomplete(string query, BookTypeEnumContract? bookType, List<int> selectedCategoryIds, List<long> selectedProjectIds)
        {
            var bookTypeEnum = Mapper.Map<BookTypeEnum?>(bookType);
            var result = m_metadataRepository.InvokeUnitOfWork(x =>
            {
                var allCategoryIds = selectedCategoryIds.Count > 0
                    ? m_categoryRepository.GetAllSubcategoryIds(selectedCategoryIds)
                    : selectedCategoryIds;
                return x.GetTitleAutocomplete(query, bookTypeEnum, allCategoryIds, selectedProjectIds, DefaultValues.AutocompleteMaxCount);
            });
            return result.ToList();
        }
    }
}
