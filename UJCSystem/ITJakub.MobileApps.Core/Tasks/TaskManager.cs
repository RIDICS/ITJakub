using System;
using System.Collections.Generic;
using AutoMapper;
using ITJakub.MobileApps.DataContracts.Tasks;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;
using ITJakub.MobileApps.DataEntities.ExternalEntities;

namespace ITJakub.MobileApps.Core.Tasks
{
    public class TaskManager
    {
        private readonly UsersRepository m_usersRepository;
        private readonly ITaskDao m_taskDataProvider;

        public TaskManager(UsersRepository usersRepository, ITaskDao taskDataProvider)
        {
            m_usersRepository = usersRepository;
            m_taskDataProvider = taskDataProvider;
        }

        public void CreateTask(long userId, int applicationId, string name, string data, string description)
        {
            var now = DateTime.UtcNow;

            var application = m_usersRepository.Load<Application>(applicationId);
            var user = m_usersRepository.Load<User>(userId);
            var task = new Task
            {
                Application = application,
                Author = user,
                CreateTime = now,
                Name = name,
                Description = description
            };

            var taskId = m_usersRepository.Create(task);
            
            ITaskEntity taskEntity = m_taskDataProvider.GetNewEntity(task.Id, applicationId, data);
            m_taskDataProvider.Save(taskEntity);
        }

        public IList<TaskDetailContract> GetTasksByApplication(int applicationId)
        {
            var tasks = m_usersRepository.GetTasksByApplication(applicationId);
            return Mapper.Map<IList<TaskDetailContract>>(tasks);
        }
        
        public TaskDataContract GetTask(long taskId)
        {
            var taskEntity = m_usersRepository.FindById<Task>(taskId);
            var taskAzure = m_taskDataProvider.FindByIdAndAppId(taskEntity.Id, taskEntity.Application.Id);

            var task = Mapper.Map<TaskDataContract>(taskEntity);
            task.Data = taskAzure.Data;

            return task;
        }

        public TaskDataContract GetTaskForGroup(long groupId)
        {
            var group = m_usersRepository.GetGroupWithTask(groupId);
            if (group == null)
                return null;

            var task = Mapper.Map<Task, TaskDataContract>(group.Task);
            if (task == null)
                return null;

            var taskEntity = m_taskDataProvider.FindByIdAndAppId(task.Id, group.Task.Application.Id);

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
