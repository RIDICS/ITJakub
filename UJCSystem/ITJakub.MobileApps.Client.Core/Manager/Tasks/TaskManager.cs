using System;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Core.Communication.Client;
using ITJakub.MobileApps.Client.Core.Communication.Error;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.Manager.Tasks
{
    public class TaskManager
    {
        private readonly MobileAppsServiceClient m_client;
        private readonly ApplicationIdManager m_applicationIdManager;
        private readonly AuthenticationManager m_authenticationManager;

        public TaskManager(MobileAppsServiceClient client, ApplicationIdManager applicationIdManager, AuthenticationManager authenticationManager)
        {
            m_client = client;
            m_applicationIdManager = applicationIdManager;
            m_authenticationManager = authenticationManager;
        }

        public async void GetTasksByApplication(ApplicationType application, Action<ObservableCollection<TaskViewModel>, Exception> callback)
        {
            try
            {
                var currentUserId = m_authenticationManager.GetCurrentUserId();
                var userId = currentUserId.HasValue ? currentUserId.Value : 0;
                var applicationId = await m_applicationIdManager.GetApplicationId(application);

                var taskList = await m_client.GetTasksByApplicationAsync(applicationId);
                await m_applicationIdManager.LoadAllApplications(); // ensure that all application IDs are loaded
                var tasks = new ObservableCollection<TaskViewModel>(taskList.Select(task => new TaskViewModel
                {
                    Id = task.Id,
                    Application = m_applicationIdManager.GetApplicationType(task.ApplicationId).Result, // all IDs are loaded -> no communication with server
                    Name = task.Name,
                    Description = task.Description,
                    CreateTime = task.CreateTime,
                    Author = new UserInfo
                    {
                        Id = task.Author.Id,
                        FirstName = task.Author.FirstName,
                        LastName = task.Author.LastName,
                        Email = task.Author.Email,
                        IsMe = userId == task.Author.Id
                    }
                }));
                callback(tasks, null);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(null, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void AssignTaskToGroup(long groupId, long taskId, Action<Exception> callback)
        {
            try
            {
                await m_client.AssignTaskToGroupAsync(groupId, taskId);
                callback(null);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public async void GetTaskForGroup(long groupId, Action<TaskViewModel, Exception> callback)
        {
            try
            {
                var result = await m_client.GetTaskForGroupAsync(groupId);
                if (result == null)
                    return;

                var task = new TaskViewModel
                {
                    Application = await m_applicationIdManager.GetApplicationType(result.ApplicationId),
                    Id = result.Id,
                    Description = result.Description,
                    Data = result.Data
                };
                callback(task, null);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(null, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetMyTasks(Action<ObservableCollection<TaskViewModel>, Exception> callback)
        {
            try
            {
                var userId = m_authenticationManager.GetCurrentUserId();
                if (!userId.HasValue)
                    return;

                var result = await m_client.GetTasksByAuthor(userId.Value);
                await m_applicationIdManager.LoadAllApplications(); // ensure that all application IDs are loaded
                var taskList = new ObservableCollection<TaskViewModel>(result.Select(task => new TaskViewModel
                {
                    Id = task.Id,
                    Application = m_applicationIdManager.GetApplicationType(task.ApplicationId).Result, // all IDs are loaded -> no communication with server
                    Name = task.Name,
                    Description = task.Description,
                    CreateTime = task.CreateTime,
                    Author = new UserInfo
                    {
                        Id = task.Author.Id,
                        FirstName = task.Author.FirstName,
                        LastName = task.Author.LastName,
                        Email = task.Author.Email,
                        IsMe = userId == task.Author.Id
                    }
                }));
                callback(taskList, null);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(null, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetTask(long taskId, Action<TaskViewModel, Exception> callback)
        {
            try
            {
                var result = await m_client.GetTaskAsync(taskId);
                if (result == null)
                    return;

                var task = new TaskViewModel
                {
                    Application = await m_applicationIdManager.GetApplicationType(result.ApplicationId),
                    Id = result.Id,
                    Description = result.Description,
                    Data = result.Data
                };
                callback(task, null);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(null, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}
