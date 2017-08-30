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
        private readonly ResourceRepository m_resourceRepository;

        public UpdateHeadwordsSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public void UpdateHeadwords(long projectId, int userId, string message, BookData bookData)
        {
            if (bookData.BookHeadwords == null)
                return;

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            foreach (var groupedHeadwordData in bookData.BookHeadwords.GroupBy(x => x.XmlEntryId))
            {
                var headwordDataList = groupedHeadwordData.OrderBy(x => x.Headword).ToList();
                
                var dbHeadword = m_resourceRepository.GetLatestHeadword(projectId, groupedHeadwordData.Key); // TODO check order of HeadwordItems

                if (dbHeadword == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = string.Empty,
                        ContentType = ContentTypeEnum.Headword,
                        ResourceType = ResourceTypeEnum.Headword,
                    };
                    CreateHeadwordResource(1, newResource, headwordDataList, user, now);
                }
                else if (IsHeadwordChanged(dbHeadword, headwordDataList))
                {
                    CreateHeadwordResource(dbHeadword.VersionNumber + 1, dbHeadword.Resource, headwordDataList, user, now);
                }
            }
        }

        private bool IsHeadwordChanged(HeadwordResource dbHeadword, IList<BookHeadwordData> headwordDataList)
        {
            var headwordData = headwordDataList.First();

            if (dbHeadword.DefaultHeadword != headwordData.DefaultHeadword ||
                dbHeadword.Sorting != headwordData.SortOrder ||
                dbHeadword.HeadwordItems.Count != headwordDataList.Count)
            {
                return true;
            }

            for (int i = 0; i < headwordDataList.Count; i++)
            {
                var headword1 = dbHeadword.HeadwordItems[i];
                var headword2 = headwordDataList[i];

                if (headword1.Headword != headword2.Headword)
                {
                    return true;
                }
            }

            return false;
        }

        private void CreateHeadwordResource(int version, Resource resource, List<BookHeadwordData> headwordDataList, User user, DateTime now)
        {
            var firstHeadwordData = headwordDataList.First();

            var newDbHeadword = new HeadwordResource
            {
                Resource = resource,
                Comment = null,
                CreateTime = now,
                CreatedByUser = user,
                VersionNumber = version,
                HeadwordItems = null, // Headword Items are create in following for-each
                ExternalId = firstHeadwordData.XmlEntryId,
                DefaultHeadword = firstHeadwordData.DefaultHeadword,
                Sorting = firstHeadwordData.SortOrder,
            };
            resource.LatestVersion = newDbHeadword;

            m_resourceRepository.Create(newDbHeadword);

            foreach (var bookHeadwordData in headwordDataList)
            {
                var newDbHeadwordItem = new HeadwordItem
                {
                    HeadwordResource = newDbHeadword,
                    Headword = bookHeadwordData.Headword,
                    HeadwordOriginal = null,
                    ResourcePage = null //TODO relation to Page
                };
                m_resourceRepository.Create(newDbHeadwordItem);
            }
        }
    }
}