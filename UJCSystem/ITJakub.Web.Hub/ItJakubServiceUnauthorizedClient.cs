using System;
using System.Reflection;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using log4net;

namespace ITJakub.Web.Hub
{
    public class ItJakubServiceUnauthorizedClient : ClientBase<IItJakubServiceUnauthorized>, IItJakubServiceUnauthorized
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserContract FindUserById(int userId)
        {
            try
            {
                return Channel.FindUserById(userId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserById failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserById timeouted with: {0}", ex);
                throw;
            }
        }

        public UserContract FindUserByUserName(string userName)
        {
            try
            {
                return Channel.FindUserByUserName(userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserByUserName failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserByUserName timeouted with: {0}", ex);
                throw;
            }
        }

        public UserContract CreateUser(UserContract user)
        {
            try
            {
                return Channel.CreateUser(user);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("CreateUser failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("CreateUser timeouted with: {0}", ex);
                throw;
            }
        }
    }
}