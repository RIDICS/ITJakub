using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Utils
{
    public class ChaptersHelper
    {
        public static List<ChapterHierarchyContract> ChapterToHierarchyContracts(IList<ChapterResource> dbResult)
        {
            var resultList = new List<ChapterHierarchyContract>(dbResult.Count);
            var chaptersDictionary = new Dictionary<long, ChapterHierarchyContract>();

            foreach (var chapterResource in dbResult)
            {
                var resultChapter = Mapper.Map<ChapterHierarchyContract>(chapterResource);
                resultChapter.SubChapters = new List<ChapterHierarchyContract>();
                chaptersDictionary.Add(resultChapter.Id, resultChapter);

                if (chapterResource.ParentResource == null)
                {
                    resultList.Add(resultChapter);
                }
                else
                {
                    var parentChapter = chaptersDictionary[chapterResource.ParentResource.Id];
                    parentChapter.SubChapters.Add(resultChapter);
                }
            }

            return resultList;
        }
    }
}
