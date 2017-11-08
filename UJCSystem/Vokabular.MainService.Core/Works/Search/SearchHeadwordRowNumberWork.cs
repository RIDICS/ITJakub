using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchHeadwordRowNumberWork : UnitOfWorkBase<long>
    {
        private readonly BookRepository m_bookRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly HeadwordRowNumberSearchRequestContract m_request;

        public SearchHeadwordRowNumberWork(BookRepository bookRepository, CategoryRepository categoryRepository, HeadwordRowNumberSearchRequestContract request) : base(bookRepository)
        {
            m_bookRepository = bookRepository;
            m_categoryRepository = categoryRepository;
            m_request = request;
        }

        protected override long ExecuteWorkImplementation()
        {
            var projectIds = m_request.Category.SelectedBookIds ?? new List<long>();
            var categoryIds = m_request.Category.SelectedCategoryIds ?? new List<int>();
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(m_request.Category.BookType);


            if (categoryIds.Count > 0)
            {
                categoryIds = m_categoryRepository.GetAllSubcategoryIds(categoryIds);
            }

            projectIds = m_bookRepository.GetProjectIds(projectIds, categoryIds, bookTypeEnum);

            return m_bookRepository.GetHeadwordRowNumber(m_request.Query, projectIds);
        }
    }
}