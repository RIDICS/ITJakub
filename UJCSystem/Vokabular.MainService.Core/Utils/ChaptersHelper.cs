using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Utils
{
    public class ChaptersHelper
    {
        public static List<T> ChapterToHierarchyContracts<T>(IList<ChapterResource> dbResult, IMapper mapper) where T : ChapterContractBase, IChapterHierarchyContract
        {
            var resultList = new List<T>(dbResult.Count);
            var chaptersDictionary = new Dictionary<long, T>();

            foreach (var chapterResource in dbResult)
            {
                var resultChapter = mapper.Map<T>(chapterResource);
                resultChapter.SubChapters = new List<IChapterHierarchyContract>();
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
