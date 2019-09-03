using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchHeadwordRowNumberWork : UnitOfWorkBase<long>
    {
        private readonly BookRepository m_bookRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly HeadwordRowNumberSearchRequestContract m_request;
        private readonly int m_userId;
        private readonly ProjectTypeEnum m_projectType;

        public SearchHeadwordRowNumberWork(BookRepository bookRepository, CategoryRepository categoryRepository,
            HeadwordRowNumberSearchRequestContract request, int userId, ProjectTypeEnum projectType) : base(bookRepository)
        {
            m_bookRepository = bookRepository;
            m_categoryRepository = categoryRepository;
            m_request = request;
            m_userId = userId;
            m_projectType = projectType;
        }

        protected override long ExecuteWorkImplementation()
        {
            var projectIds = m_request.Category.SelectedBookIds ?? new List<long>();
            var categoryIds = m_request.Category.SelectedCategoryIds ?? new List<int>();
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(m_request.Category.BookType);

            if (projectIds.Count > 0 || categoryIds.Count > 0)
            {
                if (categoryIds.Count > 0)
                {
                    categoryIds = m_categoryRepository.GetAllSubcategoryIds(categoryIds);
                }

                projectIds = m_bookRepository.GetProjectIds(bookTypeEnum, m_userId, m_projectType, projectIds, categoryIds);
            }
            else
            {
                projectIds = m_bookRepository.GetProjectIds(bookTypeEnum, m_userId, m_projectType, null, null);
            }

            return m_bookRepository.GetHeadwordRowNumber(m_request.Query, projectIds);

        }
    }
}