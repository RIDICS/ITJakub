using System.Collections.Generic;
using Nest;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SearchManager
    {
        private const string Index = "module"; //TODO rename index and type
        private const string Type = "snapshot";

        private readonly CommunicationProvider m_communicationProvider;

        public SearchManager(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

     
        public FulltextSearchResultContract SearchByCriteria(List<string> searchCriterias, List<long> projectIdList)
        {
            var client = m_communicationProvider.GetElasticClient();

            /* var response = client.Search<SnapshotResourceContract>(s => s
                 .Index(Index)
                 .Type(Type)
                 .Query(q => q
                     .Match(mq => mq
                         .Field(f => f
                             .Text)
                         .Query(string.Join(" ", searchCriterias)).Operator(Operator.Or)
                     ) 

                 )

             );
             */
            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
                .Type(Type)
                .Query(q => q
                    .Bool(b => b
                        .Should(bs => bs.Match(mq => mq.Field(f => f.Text).Query(string.Join(" ", searchCriterias)).Operator(Operator.Or)))
                    )
                )
            );
            if (response.OriginalException != null)
            {
                throw response.OriginalException;
            }
            var result = new FulltextSearchResultContract();
            result.ProjectIds = new List<long>();

            foreach (var document in response.Documents)
            {
                result.ProjectIds.Add(document.ProjectId);
            }

            return result;
        }
    }
}