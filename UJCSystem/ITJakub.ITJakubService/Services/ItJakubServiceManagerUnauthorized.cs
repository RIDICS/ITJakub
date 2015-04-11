using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Services
{
    public class ItJakubServiceManagerUnauthorized : IItJakubServiceUnauthorized
    {
        private readonly UserManager m_userManager;
        private readonly WindsorContainer m_container = Container.Current;

        public ItJakubServiceManagerUnauthorized()
        {
            m_userManager = m_container.Resolve<UserManager>();
        }

        public UserContract FindUserById(int userId)
        {
            return m_userManager.FindById(userId);
        }

        public UserContract FindUserByUserName(string userName)
        {
            return m_userManager.FindByUserName(userName);
        }

        public UserContract CreateUser(UserContract user)
        {
            return m_userManager.CreateLocalUser(user);
        }
    }
}