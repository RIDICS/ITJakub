using System.Reflection;
using Castle.Windsor.Diagnostics;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.Lemmatization.Service
{
    public class LemmatizationService:ILemmatizationService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly LemmatizationRepository m_lemmaRepository;

        public LemmatizationService()
        {
            m_lemmaRepository = Container.Current.Resolve<LemmatizationRepository>();
        }

        public string GetLemma(string word)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("test");

            return null;
        }

        public string GetStemma(string word)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("test");

            return null;
        }
    }
}