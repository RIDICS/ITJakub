using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class CreateSnapshotForImportedDataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly long m_userId;
        private readonly IList<long> m_resourceVersionIds;
        private readonly BookData m_bookData;
        private readonly string m_comment;

        public CreateSnapshotForImportedDataWork(ProjectRepository projectRepository, long projectId, long userId, IList<long> resourceVersionIds, BookData bookData, string comment) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_userId = userId;
            m_resourceVersionIds = resourceVersionIds;
            m_bookData = bookData;
            m_comment = comment;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.GetProjectWithLatestPublishedSnapshot(m_projectId);

            var resourceVersions =
                m_resourceVersionIds.Select(x => m_projectRepository.Load<ResourceVersion>(x)).ToList();

            var newDbSnapshot = new Snapshot
            {
                Project = project,
                BookTypes = null, // TODO need specify book types
                Comment = m_comment,
                CreateTime = now,
                PublishTime = now, // TODO determine if truly publish now
                User = user,
                VersionNumber = project.LatestPublishedSnapshot.VersionNumber + 1,
                ResourceVersions = resourceVersions
            };

            m_projectRepository.Create(newDbSnapshot);
        }

        private List<BookType> CreateBookType()
        {
            var xmlIdToBookType = new Dictionary<string, BookTypeEnum>();
            FillSubCategories(xmlIdToBookType, m_bookData.AllCategoriesHierarchy);

            // TODO iterate over assigned category Xml Ids
            throw new NotImplementedException();

            return null;
        }

        private void FillSubCategories(Dictionary<string, BookTypeEnum> xmlIdToBookType, List<CategoryData> categoryDataList)
        {
            foreach (var categoryData in categoryDataList)
            {
                var bookType = GetBookType(categoryData.XmlId);
                xmlIdToBookType.Add(categoryData.XmlId, bookType);

                if (categoryData.SubCategories != null)
                {
                    FillSubCategories(xmlIdToBookType, categoryData.SubCategories);
                }
            }
        }
        
        private BookTypeEnum GetBookType(string xmlId)
        {
            switch (xmlId)
            {
                case "output-dictionary":
                    return BookTypeEnum.Dictionary;
                case "output-editions":
                    return BookTypeEnum.Edition;
                case "output-text_bank":
                    return BookTypeEnum.TextBank;
                case "output-scholary_literature":
                    return BookTypeEnum.ProfessionalLiterature;
                case "output-digitized-grammar":
                    return BookTypeEnum.Grammar;
                case "output-audiobooks":
                    return BookTypeEnum.AudioBook;
                case "output-bibliography":
                    return BookTypeEnum.BibliographicalItem;
                default:
                    throw new ArgumentOutOfRangeException(nameof(xmlId), xmlId, "Unknown XML ID of category");
            }
        }
    }
}