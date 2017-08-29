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
            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            foreach (var groupedHeadwordData in bookData.BookHeadwords.GroupBy(x => x.XmlEntryId))
            {
                var headwordDataList = groupedHeadwordData.OrderBy(x => x.Headword).ToList();
                var firstHeadwordData = headwordDataList.First();

                var dbHeadword = m_resourceRepository.GetLatestHeadword(projectId, groupedHeadwordData.Key); // TODO check order of HeadwordItems

                if (dbHeadword == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = null,
                        ContentType = ContentTypeEnum.Headword,
                        ResourceType = ResourceTypeEnum.Headword,
                    };
                    var newDbHeadword = new HeadwordResource
                    {
                        Resource = newResource,
                        Comment = null,
                        CreateTime = now,
                        CreatedByUser = user,
                        VersionNumber = 1,
                        HeadwordItems = null, //TODO specify
                        ExternalId = firstHeadwordData.XmlEntryId,
                        DefaultHeadword = firstHeadwordData.DefaultHeadword,
                        Sorting = firstHeadwordData.SortOrder,
                    };
                    newResource.LatestVersion = newDbHeadword;

                    m_resourceRepository.Create(newDbHeadword);
                    //dbPageResource = newPageResource;
                    
                    // TODO create HeadwordItems!
                }
                else if (IsHeadwordChanged(dbHeadword, headwordDataList))
                {
                    // TODO create new HeadwordItemVersion
                }
                throw new NotImplementedException();
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
    }
}