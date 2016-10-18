using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
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

        public ItJakubServiceEncryptedClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
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

        public PrivateUserContract PrivateFindUserById(int userId)
        {
            try
            {
                return Channel.PrivateFindUserById(userId);
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

        public PrivateUserContract PrivateFindUserByUserName(string userName)
        {
            try
            {
                return Channel.PrivateFindUserByUserName(userName);
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

        public PrivateUserContract CreateUser(PrivateUserContract user)
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
        public bool RenewCommToken(string userName)
        {
            try
            {
                return Channel.RenewCommToken(userName);
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