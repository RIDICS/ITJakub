using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Category
{
    public class CreateCategoryWork : UnitOfWorkBase<int>
    {
        private readonly CategoryRepository m_categoryRepository;
        private readonly CategoryContract m_data;

        public CreateCategoryWork(CategoryRepository categoryRepository, CategoryContract data) : base(categoryRepository)
        {
            m_categoryRepository = categoryRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(m_data.BookType);
            var bookType = m_categoryRepository.GetBookTypeByEnum(bookTypeEnum);

            if (bookType == null)
            {
                bookType = new BookType {Type = bookTypeEnum};
                m_categoryRepository.Create(bookType);
            }

            var parentCategory = m_data.ParentCategoryId != null
                ? m_categoryRepository.FindById<DataEntities.Database.Entities.Category>(m_data.ParentCategoryId)
                : null;
            var parentPath = parentCategory != null ? parentCategory.Path : string.Empty;

            var category = new DataEntities.Database.Entities.Category
            {
                BookType = bookType,
                Description = m_data.Description,
                ExternalId = m_data.ExternalId,
                Path = string.Empty,
                ParentCategory = parentCategory
            };
            var resultId = (int) m_categoryRepository.Create(category);

            category.Path = string.Format("{0}/{1}", parentPath, resultId);
            m_categoryRepository.Update(category);

            return resultId;
        }
    }
}