using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using log4net;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    public class LemmatizationServiceClient : ClientBase<ILemmatizationService>, ILemmatizationService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public string GetLemma(string word)
        {
            try
            {
                return Channel.GetLemma(word);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public string GetStemma(string word)
        {
            throw new System.NotImplementedException();
        }

        private string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }
    }
}
