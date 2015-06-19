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

        public IList<TaskDetailContract> GetTasksByApplication(int applicationId)
        {
            var tasks = m_usersRepository.GetTasksByApplication(applicationId);
            return Mapper.Map<IList<TaskDetailContract>>(tasks);
        }

        public TaskDataContract GetTaskForGroup(long groupId)
        {
            var group = m_usersRepository.GetGroupWithTask(groupId);
            if (group == null)
                return null;

            var task = Mapper.Map<Task, TaskDataContract>(group.Task);
            if (task == null)
                return null;

            var taskEntity = m_azureTableTaskDao.FindByRowAndPartitionKey(Convert.ToString(task.Id),
                Convert.ToString(group.Task.Application.Id));

            if (taskEntity != null)
                task.Data = taskEntity.Data;

            return task;
        }

        public IList<TaskDetailContract> GetTasksByAuthor(long userId)
        {
            var taskList = m_usersRepository.GetTasksByAuthor(userId);
            return Mapper.Map<IList<TaskDetailContract>>(taskList);
        }
    }
}
