using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.ITJakubService.DataContracts.Clients
{
    public class ItJakubServiceEncryptedClient : ClientBase<IItJakubServiceEncrypted>, IItJakubServiceEncrypted
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ItJakubServiceEncryptedClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public UserContract FindUserById(int userId)
        {
            try
            {
                return Channel.FindUserById(userId);
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
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
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
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
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
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }
        public bool RenewCommToken(string userName, string password)
        {
            try
            {
                return Channel.RenewCommToken(userName, password);
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
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }



        private string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }

     
    }
}