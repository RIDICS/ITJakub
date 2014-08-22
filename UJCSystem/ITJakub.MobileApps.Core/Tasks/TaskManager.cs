using System;
using System.Collections.Generic;
using AutoMapper;
using ITJakub.MobileApps.DataContracts.Tasks;
using ITJakub.MobileApps.DataEntities.AzureTables.Daos;
using ITJakub.MobileApps.DataEntities.AzureTables.Entities;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Tasks
{
    public class TaskManager
    {
        private readonly UsersRepository m_usersRepository;
        private readonly AzureTableTaskDao m_azureTableTaskDao;

        public TaskManager(UsersRepository usersRepository, AzureTableTaskDao azureTableTaskDao)
        {
            m_usersRepository = usersRepository;
            m_azureTableTaskDao = azureTableTaskDao;
        }

        public void CreateTask(long userId, int applicationId, string name, string data)
        {
            var now = DateTime.UtcNow;

            var application = m_usersRepository.Load<Application>(applicationId);
            var user = m_usersRepository.Load<User>(userId);
            var task = new Task
            {
                Application = application,
                Author = user,
                CreateTime = now,
                Name = name
            };

            var taskId = m_usersRepository.Create(task);

            var taskEntity = new TaskEntity(Convert.ToString(taskId), Convert.ToString(applicationId), data);
            m_azureTableTaskDao.Create(taskEntity);
        }

        public IList<TaskContract> GetTasksByApplication(int applicationId)
        {
            var tasks = m_usersRepository.GetTasksByApplication(applicationId);

            foreach (var task in tasks) //TODO try to find some better way how to fill Data property
            {
                TaskEntity taskEntity = m_azureTableTaskDao.FindByRowAndPartitionKey(Convert.ToString(task.Id), Convert.ToString(applicationId));
                if (taskEntity != null)
                    task.Data = taskEntity.Data;
            }
            return Mapper.Map<IList<TaskContract>>(tasks);
        }
    }
}
