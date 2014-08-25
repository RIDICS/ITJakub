using System;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
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
                var applicationId = await m_applicationIdManager.GetApplicationIdAsync(application);

                var taskList = await m_client.GetTasksByApplicationAsync(applicationId);
                var tasks = new ObservableCollection<TaskViewModel>(taskList.Select(task => new TaskViewModel
                {
                    Id = task.Id,
                    Name = task.Name,
                    CreateTime = task.CreateTime,
                    Data = task.Data,
                    Author = new AuthorInfo
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
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }
    }
}
