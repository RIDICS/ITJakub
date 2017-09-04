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

        public UpdateHeadwordsSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public void UpdateHeadwords(long projectId, long bookVersionId, int userId, string message, BookData bookData)
        {
            if (bookData.BookHeadwords == null)
                return;

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);
            var bookVersion = m_resourceRepository.Load<BookVersionResource>(bookVersionId);

            var dbHeadwords = new Dictionary<string, HeadwordResource>();
            IList<HeadwordResource> tempDbHeadwords;
            string lastExternalId = null;
            var page = 0;
            do
            {
                tempDbHeadwords = m_resourceRepository.GetProjectLatestHeadwordPage(projectId, page * BatchSize, BatchSize); // TODO check order of HeadwordItems
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
                    CreateHeadwordResource(1, newResource, headwordDataList, user, now, bookVersion);
                }
                else if (IsHeadwordChanged(dbHeadword, headwordDataList))
                {
                    CreateHeadwordResource(dbHeadword.VersionNumber + 1, dbHeadword.Resource, headwordDataList, user, now, bookVersion);
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

                if (headword1.Headword != headword2.Headword ||
                    headword1.HeadwordOriginal != headword2.HeadwordOriginal)
                {
                    return true;
                }
            }

            return false;
        }

        private void CreateHeadwordResource(int version, Resource resource, List<BookHeadwordData> headwordDataList, User user, DateTime now, BookVersionResource bookVersion)
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

            foreach (var bookHeadwordData in headwordDataList)
            {
                var newDbHeadwordItem = new HeadwordItem
                {
                    HeadwordResource = newDbHeadword,
                    Headword = bookHeadwordData.Headword,
                    HeadwordOriginal = bookHeadwordData.HeadwordOriginal,
                    ResourcePage = null //TODO relation to Page
                };
                m_resourceRepository.Create(newDbHeadwordItem);
            }
        }
    }
}