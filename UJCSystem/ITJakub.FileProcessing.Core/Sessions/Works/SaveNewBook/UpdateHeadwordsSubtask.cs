using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateHeadwordsSubtask
    {
        private const int BatchSize = 1000;
        private readonly ResourceRepository m_resourceRepository;
        private List<long> m_importedResourceVersionIds;

        public UpdateHeadwordsSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public List<long> ImportedResourceVersionIds => m_importedResourceVersionIds;

        private Dictionary<string, PageResource> CreateImageToPageResourceDictionary(BookData bookData, List<PageResource> dbPageResources)
        {
            if (dbPageResources == null || bookData.Pages == null)
                return new Dictionary<string, PageResource>();

            var dbPagesByPosition = new PageResource[dbPageResources.Count + 1];
            foreach (var dbPage in dbPageResources)
            {
                dbPagesByPosition[dbPage.Position] = dbPage;
            }

            var dbPagesByImage = new Dictionary<string, PageResource>();
            foreach (var pageWithImageData in bookData.Pages.Where(x => x.Image != null))
            {
                var dbPage = dbPagesByPosition[pageWithImageData.Position];
                dbPagesByImage.Add(pageWithImageData.Image, dbPage);
            }

            return dbPagesByImage;
        }

        public void UpdateHeadwords(long projectId, long bookVersionId, int userId, BookData bookData, List<PageResource> dbPageResources)
        {
            m_importedResourceVersionIds = new List<long>();
            if (bookData.BookHeadwords == null)
                return;

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);
            var bookVersion = m_resourceRepository.Load<BookVersionResource>(bookVersionId);

            // Load all project headwords from database
            var dbPagesByImage = CreateImageToPageResourceDictionary(bookData, dbPageResources);
            var dbHeadwords = new Dictionary<string, HeadwordResource>();
            IList<HeadwordResource> tempDbHeadwords;
            string lastExternalId = null;
            var page = 0;
            do
            {
                tempDbHeadwords = m_resourceRepository.GetProjectLatestHeadwordPage(projectId, page * BatchSize, BatchSize); // returns HeadwordResource list without fetched HeadwordItem
                foreach (var tempDbHeadword in tempDbHeadwords)
                {
                    if (tempDbHeadword.ExternalId != lastExternalId)
                    {
                        dbHeadwords.Add(tempDbHeadword.ExternalId, tempDbHeadword);
                        lastExternalId = tempDbHeadword.ExternalId;
                    }
                }
                page++;
            } while (tempDbHeadwords.Count > 0);

            // Create new Headword with ResourceVersion
            foreach (var groupedHeadwordData in bookData.BookHeadwords.GroupBy(x => x.XmlEntryId))
            {
                var headwordDataList = groupedHeadwordData.OrderBy(x => x.HeadwordOriginal).ToList();
                
                HeadwordResource dbHeadword;
                if (!dbHeadwords.TryGetValue(groupedHeadwordData.Key, out dbHeadword))
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = string.Empty,
                        ContentType = ContentTypeEnum.Headword,
                        ResourceType = ResourceTypeEnum.Headword,
                    };
                    CreateHeadwordResource(1, newResource, headwordDataList, user, now, bookVersion, dbPagesByImage);
                }
                else if (IsHeadwordChanged(dbHeadword, headwordDataList, bookVersionId))
                {
                    CreateHeadwordResource(dbHeadword.VersionNumber + 1, dbHeadword.Resource, headwordDataList, user, now, bookVersion, dbPagesByImage);
                }
                else
                {
                    m_importedResourceVersionIds.Add(dbHeadword.Id); // no new ResourceVersion, but imported
                }
            }
        }

        private bool IsHeadwordChanged(HeadwordResource dbHeadword, IList<BookHeadwordData> headwordDataList, long bookVersionId)
        {
            var headwordData = headwordDataList.First();

            if (dbHeadword.DefaultHeadword != headwordData.DefaultHeadword ||
                dbHeadword.Sorting != headwordData.SortOrder ||
                dbHeadword.BookVersion?.Id != bookVersionId ||
                dbHeadword.HeadwordItems.Count != headwordDataList.Count) // lazy fetching collection
            {
                return true;
            }

            var dbHeadwordItems = dbHeadword.HeadwordItems.OrderBy(x => x.HeadwordOriginal).ToArray();
            for (int i = 0; i < headwordDataList.Count; i++)
            {
                var headword1 = dbHeadwordItems[i];
                var headword2 = headwordDataList[i];

                if (headword1.Headword != headword2.Headword ||
                    headword1.HeadwordOriginal != headword2.HeadwordOriginal)
                {
                    return true;
                }
            }

            return false;
        }

        private void CreateHeadwordResource(int version, Resource resource, List<BookHeadwordData> headwordDataList, User user, DateTime now, BookVersionResource bookVersion, Dictionary<string, PageResource> dbPagesByImage)
        {
            var firstHeadwordData = headwordDataList.First();

            var newDbHeadword = new HeadwordResource
            {
                Resource = resource,
                BookVersion = bookVersion,
                Comment = null,
                CreateTime = now,
                CreatedByUser = user,
                VersionNumber = version,
                HeadwordItems = null, // Headword Items are created in following for-each
                ExternalId = firstHeadwordData.XmlEntryId,
                DefaultHeadword = firstHeadwordData.DefaultHeadword,
                Sorting = firstHeadwordData.SortOrder,
            };
            resource.LatestVersion = newDbHeadword;

            m_resourceRepository.Create(newDbHeadword);
            m_importedResourceVersionIds.Add(newDbHeadword.Id);

            foreach (var bookHeadwordData in headwordDataList)
            {
                var dbPage = GetPageResourceByImage(dbPagesByImage, bookHeadwordData.Image);
                var newDbHeadwordItem = new HeadwordItem
                {
                    HeadwordResource = newDbHeadword,
                    Headword = bookHeadwordData.Headword,
                    HeadwordOriginal = bookHeadwordData.HeadwordOriginal,
                    ResourcePage = dbPage?.Resource
                };
                m_resourceRepository.Create(newDbHeadwordItem);
            }
        }

        private PageResource GetPageResourceByImage(Dictionary<string, PageResource> dbPagesByImage, string image)
        {
            if (image == null)
                return null;

            dbPagesByImage.TryGetValue(image, out var dbPage);
            return dbPage;
        }
    }
}