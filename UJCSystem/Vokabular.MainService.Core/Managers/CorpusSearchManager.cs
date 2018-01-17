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
        private const int CorpusMaxCount = 200;
        private const int CorpusDefaultCount = 50;
        private const int CorpusDefaultStart = 0;
        private readonly MetadataRepository m_metadataRepository;
        private readonly BookRepository m_bookRepository;

        public CorpusSearchManager(MetadataRepository metadataRepository, BookRepository bookRepository)
        {
            m_metadataRepository = metadataRepository;
            m_bookRepository = bookRepository;
        }

        public int GetCorpusStart(int? start)
        {
            return start ?? CorpusDefaultStart;
        }

        public int GetCorpusCount(int? count)
        {
            return count != null ? Math.Min(count.Value, CorpusMaxCount) : CorpusDefaultCount;
        }

        public List<CorpusSearchResultContract> GetCorpusSearchResultByStandardIds(List<CorpusSearchResultData> list)
        {
            var projectIds = list.Select(x => x.ProjectId).Distinct().ToList();
            var dbProjects = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByProjectIds(projectIds));
            var bookDictionary = dbProjects.ToDictionary(x => x.Resource.Project.Id);
            
            // Load all pages in one transaction
            var orderedPageResourceList = m_bookRepository.InvokeUnitOfWork(repository =>
            {
                var result = new List<PageResource>();
                foreach (var corpusSearchResultData in list)
                {
                    var page = repository.GetPagesByTextExternalId(new[] { corpusSearchResultData.PageResultContext.TextExternalId }, corpusSearchResultData.ProjectId);
                    result.Add(page.First());
                }

                return result;
            });

            // Process results
            var orderedResultList = new List<CorpusSearchResultContract>();
            for (int i = 0; i < list.Count; i++)
            {
                var corpusResultData = list[i];
                var pageInfo = orderedPageResourceList[i];
                var projectMetadata = bookDictionary[corpusResultData.ProjectId];

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
                orderedResultList.Add(corpusItemContract);
            }

            return orderedResultList;
        }

        public List<CorpusSearchResultContract> GetCorpusSearchResultByExternalIds(List<CorpusSearchResultData> list)
        {
            var projectExternalIds = list.Select(x => x.ProjectExternalId);
            var dbProjects = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByProjectExternalIds(projectExternalIds));
            var bookDictionary = dbProjects.ToDictionary(x => x.Resource.Project.ExternalId);

            // Load all pages in one transaction
            var orderedPageResourceList = m_bookRepository.InvokeUnitOfWork(repository =>
            {
                var result = new List<PageResource>();
                foreach (var corpusSearchResultData in list)
                {
                    var page = repository.GetPagesByTextExternalId(new [] {corpusSearchResultData.PageResultContext.TextExternalId}, null, corpusSearchResultData.ProjectExternalId);
                    result.Add(page.First());
                }

                return result;
            });

            // Process results
            var orderedResultList = new List<CorpusSearchResultContract>();
            for (int i = 0; i < list.Count; i++)
            {
                var corpusResultData = list[i];
                var pageInfo = orderedPageResourceList[i];
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
                orderedResultList.Add(corpusItemContract);
            }

            return orderedResultList;
        }
    }
}
