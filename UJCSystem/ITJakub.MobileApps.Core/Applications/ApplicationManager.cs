using System;
using System.Collections.Generic;
using System.ServiceModel;
using AutoMapper;
using ITJakub.MobileApps.DataContracts;
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
        private readonly AzureTableIdGenerator m_idGenerator;

        public ApplicationManager(ApplicationRepository applicationRepository, UsersRepository usersRepository, AzureTableSynchronizedObjectDao azureTableSynchronizedObjectDao, AzureTableIdGenerator idGenerator)
        {
            m_applicationRepository = applicationRepository;
            m_usersRepository = usersRepository;
            m_azureTableSynchronizedObjectDao = azureTableSynchronizedObjectDao;
            m_idGenerator = idGenerator;
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            if (group.State != GroupState.Running)
            {
                throw new FaultException<ApplicationNotRunningFault>(new ApplicationNotRunningFault(), "Application is not in the Running state.");
            }

            var syncObjectEntity = new SynchronizedObjectEntity(m_idGenerator.GetNewId(), Convert.ToString(groupId), synchronizedObject.Data);
            m_azureTableSynchronizedObjectDao.Create(syncObjectEntity);

            var now = DateTime.UtcNow;

            var application = m_applicationRepository.Load<Application>(applicationId);                
            var user = m_usersRepository.Load<User>(userId);
            
            SynchronizedObject deSyncObject = Mapper.Map<SynchronizedObject>(synchronizedObject);
            deSyncObject.Application = application;
            deSyncObject.Author = user;
            deSyncObject.Group = group;
            deSyncObject.CreateTime = now;
            deSyncObject.RowKey = syncObjectEntity.RowKey;

            m_applicationRepository.Create(deSyncObject);
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            if (group.State >= GroupState.Paused)
            {
                throw new FaultException<ApplicationNotRunningFault>(new ApplicationNotRunningFault(), "Application is paused or closed.");
            }

            var syncObjs = m_applicationRepository.GetSynchronizedObjects(groupId, applicationId, objectType, since);

            foreach (SynchronizedObject syncObj in syncObjs) //TODO try to find some better way how to fill Data property
            {
                SynchronizedObjectEntity syncObjEntity = m_azureTableSynchronizedObjectDao.FindByRowAndPartitionKey(syncObj.RowKey,
                    Convert.ToString(syncObj.Group.Id));

                syncObj.Data = syncObjEntity.Data;
            }

            return Mapper.Map<IList<SynchronizedObjectResponseContract>>(syncObjs);
        }

        public IList<ApplicationContract> GetAllApplication()
        {
            var apps = m_applicationRepository.GetAllApplication();
            return Mapper.Map<IList<ApplicationContract>>(apps);
        }

        public void DeleteSynchronizedObjects(long groupId, IEnumerable<string> rowKeys)
        {
            foreach (var rowKey in rowKeys)
            {
                m_azureTableSynchronizedObjectDao.Delete(rowKey, Convert.ToString(groupId));
            }
        }

        public SynchronizedObjectResponseContract GetLatestSynchronizedObject(long groupId, int applicationId, string objectType, DateTime since)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            if (group.State >= GroupState.Paused)
            {
                throw new FaultException<ApplicationNotRunningFault>(new ApplicationNotRunningFault(), "Application is paused or closed.");
            }

            var syncObj = m_applicationRepository.GetLatestSynchronizedObject(groupId, applicationId, objectType, since);
            if (syncObj == null)
                return null;

            SynchronizedObjectEntity syncObjEntity = m_azureTableSynchronizedObjectDao.FindByRowAndPartitionKey(syncObj.RowKey, Convert.ToString(syncObj.Group.Id));
            syncObj.Data = syncObjEntity.Data;

            return Mapper.Map<SynchronizedObjectResponseContract>(syncObj);
        }
    }
}