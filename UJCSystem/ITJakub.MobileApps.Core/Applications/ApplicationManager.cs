using System;
using System.Globalization;
using AutoMapper;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataEntities.AzureTables.Daos;
using ITJakub.MobileApps.DataEntities.AzureTables.Entities;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Applications
{
    public class ApplicationManager
    {
        private readonly ApplicationRepository m_applicationRepository;
        private readonly AzureTableSynchronizedObjectDao m_azureTableSynchronizedObjectDao;
        private readonly UsersRepository m_usersRepository;

        public ApplicationManager(ApplicationRepository applicationRepository, UsersRepository usersRepository,
            AzureTableSynchronizedObjectDao azureTableSynchronizedObjectDao)
        {
            m_applicationRepository = applicationRepository;
            m_usersRepository = usersRepository;
            m_azureTableSynchronizedObjectDao = azureTableSynchronizedObjectDao;
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            var now = DateTime.UtcNow;

            var application = m_applicationRepository.Load(applicationId);                
            var user = m_usersRepository.Load<User>(userId);
            var group = m_usersRepository.Load<Group>(groupId);
            SynchronizedObject deSyncObject = Mapper.Map<SynchronizedObject>(synchronizedObject);
            deSyncObject.Application = application;
            deSyncObject.Author = user;
            deSyncObject.Group = group;
            deSyncObject.CreateTime = now;

            object syncObjId = m_applicationRepository.Create(deSyncObject);
            m_azureTableSynchronizedObjectDao.Create(new SynchronizedObjectEntity(syncObjId.ToString(), Convert.ToString(groupId), synchronizedObject.Data));
         
        }
    }
}