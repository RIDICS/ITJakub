using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceTermClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceTermClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public List<TermCategoryDetailContract> GetTermCategoriesWithTerms()
        {
            try
            {
                var result = m_client.Get<List<TermCategoryDetailContract>>("term/category/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
