using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class CreateSnapshotForImportedDataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly int m_userId;
        private readonly IList<long> m_resourceVersionIds;
        private readonly BookData m_bookData;
        private readonly string m_comment;
        private readonly long m_bookVersionId;
        private long m_snapshotId;

        public CreateSnapshotForImportedDataWork(ProjectRepository projectRepository, long projectId, int userId, IList<long> resourceVersionIds, BookData bookData, string comment, long bookVersionId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_userId = userId;
            m_resourceVersionIds = resourceVersionIds;
            m_bookData = bookData;
            m_comment = comment;
            m_bookVersionId = bookVersionId;
        }

        public List<BookTypeEnum> BookTypes { get; private set; }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.Load<Project>(m_projectId);
            var bookVersionResource = m_projectRepository.Load<BookVersionResource>(m_bookVersionId);
            var latestSnapshot = m_projectRepository.GetLatestSnapshot(m_projectId);

            var resourceVersions =
                m_resourceVersionIds.Select(x => m_projectRepository.Load<ResourceVersion>(x)).ToList();

            var bookTypes = CreateBookTypes();
            var versionNumber = latestSnapshot != null ? latestSnapshot.VersionNumber : 0;
            var newDbSnapshot = new Snapshot
            {
                Project = project,
                BookTypes = bookTypes,
                DefaultBookType = bookTypes.FirstOrDefault(),
                Comment = m_comment,
                CreateTime = now,
                PublishTime = now,
                CreatedByUser = user,
                VersionNumber = versionNumber + 1,
                BookVersion = bookVersionResource,
                ResourceVersions = resourceVersions
            };

            m_snapshotId = (long)m_projectRepository.Create(newDbSnapshot);

            project.LatestPublishedSnapshot = newDbSnapshot;
            m_projectRepository.Update(project);
        }

        private List<BookType> CreateBookTypes()
        {
            var xmlIdToBookType = new Dictionary<string, BookTypeEnum>();
            FillCategories(xmlIdToBookType, m_bookData.AllCategoriesHierarchy);

            var bookTypes = new List<BookTypeEnum>();
            var resultList = new List<BookType>();
            foreach (var categoryXmlId in m_bookData.CategoryXmlIds)
            {
                if (!xmlIdToBookType.TryGetValue(categoryXmlId, out var bookTypeEnum))
                    continue;

                if (bookTypes.Contains(bookTypeEnum))
                    continue;

                bookTypes.Add(bookTypeEnum);

                var dbBookType = m_projectRepository.GetBookTypeByEnum(bookTypeEnum);
                if (dbBookType == null)
                {
                    dbBookType = new BookType
                    {
                        Type = bookTypeEnum
                    };
                    m_projectRepository.Create(dbBookType);
                }

                resultList.Add(dbBookType);
            }

            BookTypes = bookTypes;
            return resultList;
        }

        private void FillCategories(Dictionary<string, BookTypeEnum> xmlIdToBookType, List<CategoryData> categoryDataList)
        {
            foreach (var categoryData in categoryDataList)
            {
                var bookType = GetBookType(categoryData.XmlId);
                xmlIdToBookType.Add(categoryData.XmlId, bookType);

                if (categoryData.SubCategories != null)
                {
                    FillSubcategories(bookType, xmlIdToBookType, categoryData.SubCategories);
                }
            }
        }

        private void FillSubcategories(BookTypeEnum bookType, Dictionary<string, BookTypeEnum> xmlIdToBookType,
            List<CategoryData> categoryDataList)
        {
            foreach (var categoryData in categoryDataList)
            {
                xmlIdToBookType.Add(categoryData.XmlId, bookType);

                if (categoryData.SubCategories != null)
                {
                    FillCategories(xmlIdToBookType, categoryData.SubCategories);
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

        public long SnapshotId
        {
            get { return m_snapshotId; }
        }
    }
}