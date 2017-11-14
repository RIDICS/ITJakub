using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace Vokabular.MainService.Core.Managers
{
    public class CorpusSearchManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly BookRepository m_bookRepository;

        public CorpusSearchManager(MetadataRepository metadataRepository, BookRepository bookRepository)
        {
            m_metadataRepository = metadataRepository;
            m_bookRepository = bookRepository;
        }

        public List<CorpusSearchResultContract> GetCorpusSearchResultByStandardIds(List<CorpusSearchResultData> list)
        {
            throw new NotImplementedException();
        }

        public List<CorpusSearchResultContract> GetCorpusSearchResultByExternalIds(List<CorpusSearchResultData> list)
        {
            var projectExternalIds = list.Select(x => x.ProjectExternalId);
            var dbProjects = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByProjectExternalIds(projectExternalIds));
            var bookDictionary = dbProjects.ToDictionary(x => x.Resource.Project.ExternalId);

            var pageResourceList = m_bookRepository.InvokeUnitOfWork(repository =>
            {
                var result = new List<PageResource>();
                foreach (var corpusSearchResultData in list)
                {
                    var page = m_bookRepository.GetPagesByTextExternalId(new [] {corpusSearchResultData.PageResultContext.TextExternalId}, null, corpusSearchResultData.ProjectExternalId);
                    result.Add(page.First());
                }

                return result;
            });

            var resultList = new List<CorpusSearchResultContract>();
            for (int i = 0; i < list.Count; i++)
            {
                var corpusResultData = list[i];
                var pageInfo = pageResourceList[i];
                var projectMetadata = bookDictionary[corpusResultData.ProjectExternalId];

                var pageContextContract = Mapper.Map<PageWithContextContract>(pageInfo);
                pageContextContract.ContextStructure = corpusResultData.PageResultContext.ContextStructure;

                var corpusItemContract = new CorpusSearchResultContract
                {
                    PageResultContext = pageContextContract,
                    Author = projectMetadata.AuthorsLabel,
                    BookId = projectMetadata.Resource.Project.Id,
                    OriginDate = projectMetadata.OriginDate,
                    RelicAbbreviation = projectMetadata.RelicAbbreviation,
                    SourceAbbreviation = projectMetadata.SourceAbbreviation,
                    Title = projectMetadata.Title,
                    Notes = corpusResultData.Notes,
                    BibleVerseResultContext = corpusResultData.BibleVerseResultContext,
                    VerseResultContext = corpusResultData.VerseResultContext,
                };
                resultList.Add(corpusItemContract);
            }

            return resultList;
        }
    }
}
