using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.ProjectMetadata;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectMetadataManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly CategoryRepository m_categoryRepository;
        private readonly IMapper m_mapper;

        public ProjectMetadataManager(MetadataRepository metadataRepository,
            AuthenticationManager authenticationManager, AuthorizationManager authorizationManager,
            CategoryRepository categoryRepository, IMapper mapper)
        {
            m_metadataRepository = metadataRepository;
            m_authenticationManager = authenticationManager;
            m_authorizationManager = authorizationManager;
            m_categoryRepository = categoryRepository;
            m_mapper = mapper;
        }

        public ProjectMetadataResultContract GetProjectMetadata(long projectId, GetProjectMetadataParameter parameters)
        {
            var work = new GetLatestProjectMetadataWork(m_metadataRepository, projectId, parameters);
            var result = work.Execute();
            var resultContract = result != null
                ? m_mapper.Map<ProjectMetadataResultContract>(result)
                : new ProjectMetadataResultContract();

            if (result != null && parameters.IsAnyAdditionalParameter())
            {
                var project = result.Resource.Project;

                if (parameters.IncludeAuthor)
                {
                    resultContract.AuthorList = m_mapper.Map<List<OriginalAuthorContract>>(project.Authors);
                }
                if (parameters.IncludeResponsiblePerson)
                {
                    resultContract.ResponsiblePersonList = m_mapper.Map<List<ProjectResponsiblePersonContract>>(project.ResponsiblePersons);
                }
                if (parameters.IncludeKind)
                {
                    resultContract.LiteraryKindList = m_mapper.Map<List<LiteraryKindContract>>(project.LiteraryKinds);
                }
                if (parameters.IncludeGenre)
                {
                    resultContract.LiteraryGenreList = m_mapper.Map<List<LiteraryGenreContract>>(project.LiteraryGenres);
                }
                if (parameters.IncludeOriginal)
                {
                    resultContract.LiteraryOriginalList = m_mapper.Map<List<LiteraryOriginalContract>>(project.LiteraryOriginals);
                }
                if (parameters.IncludeKeyword)
                {
                    resultContract.KeywordList = m_mapper.Map<List<KeywordContract>>(project.Keywords);
                }
                if (parameters.IncludeCategory)
                {
                    resultContract.CategoryList = m_mapper.Map<List<CategoryContract>>(project.Categories);
                }
            }

            return resultContract;
        }

        public long CreateNewProjectMetadataVersion(long projectId, ProjectMetadataContract data)
        {
            var resultId = new CreateNewMetadataVersionWork(m_metadataRepository, projectId, data, m_authenticationManager.GetCurrentUserId()).Execute();
            return resultId;
        }

        public List<string> GetPublisherAutocomplete(string query)
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetPublisherAutocomplete(query, DefaultValues.AutocompleteCount));
            return result.ToList();
        }

        public List<string> GetCopyrightAutocomplete(string query)
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetCopyrightAutocomplete(query, DefaultValues.AutocompleteCount));
            return result.ToList();
        }

        public List<string> GetManuscriptRepositoryAutocomplete(string query)
        {
            var result = m_metadataRepository.InvokeUnitOfWork(x => x.GetManuscriptRepositoryAutocomplete(query, DefaultValues.AutocompleteCount));
            return result.ToList();
        }

        public List<string> GetTitleAutocomplete(string query, BookTypeEnumContract? bookType, ProjectTypeContract? projectType,
            List<int> selectedCategoryIds, List<long> selectedProjectIds)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            var bookTypeEnum = m_mapper.Map<BookTypeEnum?>(bookType);
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum?>(projectType);
            var result = m_metadataRepository.InvokeUnitOfWork(x =>
            {
                var allCategoryIds = selectedCategoryIds.Count > 0
                    ? m_categoryRepository.GetAllSubcategoryIds(selectedCategoryIds)
                    : selectedCategoryIds;
                return x.GetTitleAutocomplete(query, bookTypeEnum, projectTypeEnum, allCategoryIds, selectedProjectIds, DefaultValues.AutocompleteCount, userId);
            });
            return result.ToList();
        }
    }
}
