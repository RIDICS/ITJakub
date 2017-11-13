using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace Vokabular.MainService.Core.Managers
{
    public class CorpusSearchManager
    {
        private readonly MetadataRepository m_metadataRepository;

        public CorpusSearchManager(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
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

            // TODO load book details

            foreach (var corpusSearchResultData in list)
            {
                // TODO load page details inside foreach because non-unique TextResource.ExternalId
                //corpusSearchResultData.PageResultContext.
            }

            throw new NotImplementedException();
        }
    }
}
