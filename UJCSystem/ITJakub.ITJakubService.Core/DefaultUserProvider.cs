using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;

namespace ITJakub.ITJakubService.Core
{
    public class DefaultUserProvider
    {        
        private readonly string m_registeredUsersGroupName;        
        private readonly string m_unRegisteredUsersGroupName;


        private readonly UserRepository m_repository;
        private readonly string m_unregisteredUserName;

        public DefaultUserProvider(string unregisteredUserName, string unregisteredUsersGroupName,string registeredUsersGroupName, UserRepository repository)
        {
            m_unregisteredUserName = unregisteredUserName;
            m_unRegisteredUsersGroupName = unregisteredUsersGroupName;
            m_registeredUsersGroupName = registeredUsersGroupName;


            m_repository = repository;
        }

        public User GetDefaultUser()
        {
            return m_repository.GetVirtualUserForUnregisteredUsersOrCreate(m_unregisteredUserName, GetDefaultUnRegisteredUserGroup());
        }

        public string GetDefaultUserName()
        {
            return m_unregisteredUserName;
        }

        public Group GetDefaultUnRegisteredUserGroup()
        {
            return m_repository.GetDefaultGroupOrCreate(m_unRegisteredUsersGroupName);
        }

        


        public Group GetDefaultRegisteredUserGroup()
        {
            return m_repository.GetDefaultGroupOrCreate(m_registeredUsersGroupName);
        }


    }
}