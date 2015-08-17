using System.Collections.Generic;
using System.Reflection;
using Castle.Windsor;
using ITJakub.Lemmatization.Core;
using ITJakub.Lemmatization.Shared.Contracts;
using log4net;

namespace ITJakub.Lemmatization.Service
{
    public class LemmatizationService : ILemmatizationService
    {
        private readonly LemmatizationManager m_lemmatizationManager;
        private readonly WindsorContainer m_container = Container.Current;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public LemmatizationService()
        {
            m_lemmatizationManager = m_container.Resolve<LemmatizationManager>();
        }

        public string GetLemma(string word)
        {
            return null;
        }

        public string GetStemma(string word)
        {

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("test");

            return null;
        }

        public IList<LemmatizationTypeaheadContract> GetTypeaheadToken(string query)
        {
            return m_lemmatizationManager.GetTypeaheadToken(query);
        }
    }
}