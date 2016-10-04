using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Services
{
    public class ItJakubServiceEncryptedManager : IItJakubServiceEncrypted
    {
        private readonly WindsorContainer m_container = Container.Current;
        private readonly UserManager m_userManager;
        private readonly AuthenticationManager m_authenticationManager;


        public ItJakubServiceEncryptedManager()
        {
            m_userManager = m_container.Resolve<UserManager>();
            m_authenticationManager = m_container.Resolve<AuthenticationManager>();
        }

        public UserContract FindUserById(int userId)
        {
            return m_userManager.GetUserDetail(userId);
        }

        public PrivateUserContract PrivateFindUserById(int userId)
        {
            return m_userManager.GetPrivateUserDetail(userId);
        }

        public UserContract FindUserByUserName(string userName)
        {
            return m_userManager.FindByUserName(userName);
        }

        public PrivateUserContract PrivateFindUserByUserName(string userName)
        {
            return m_userManager.PrivateFindByUserName(userName);
        }

        public PrivateUserContract CreateUser(PrivateUserContract user)
        {
            return m_userManager.CreateLocalUser(user);
        }

        public bool RenewCommToken(string username)
        {
            return m_authenticationManager.RenewCommToken(username);
        }
    }
}