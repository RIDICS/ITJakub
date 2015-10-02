using System;
using System.Collections.Generic;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.News;
using ITJakub.Shared.Contracts.Notes;

namespace ITJakub.ITJakubService.Services
{
    public class ItJakubServiceEncryptedManager : IItJakubServiceEncrypted
    {
        private readonly WindsorContainer m_container = Container.Current;
        private readonly UserManager m_userManager;


        public ItJakubServiceEncryptedManager()
        {
            m_userManager = m_container.Resolve<UserManager>();         
        }

        public UserContract FindUserById(int userId)
        {
            return m_userManager.GetUserDetail(userId);
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