﻿using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CategoryManagement
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
            var parentCategory = m_data.ParentCategoryId != null
                ? m_categoryRepository.FindById<Category>(m_data.ParentCategoryId)
                : null;
            var parentPath = parentCategory != null ? parentCategory.Path : string.Empty;

            var category = new Category
            {
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