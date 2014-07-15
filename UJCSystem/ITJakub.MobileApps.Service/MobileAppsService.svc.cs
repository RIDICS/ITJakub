using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.RequestObjects;
using ITJakub.MobileApps.DataContracts.ResponseObjects;

namespace ITJakub.MobileApps.Service
{
    public class MobileAppsService : IMobileAppsService
    {
        private readonly IMobileAppsService m_manager;

        public MobileAppsService()
        {
            m_manager = Container.Current.Resolve<IMobileAppsService>();
        }

        public string TestMethod(string test)
        {
            return m_manager.TestMethod(test);//Handle exceptions and different managers here
        }   

        public CreateInstitutionResponse CreateInstitution(Institution institution)
        {
            return m_manager.CreateInstitution(institution);
        }

        public InstitutionDetailsResponse GetInstitutionDetails(string institutionId)
        {
            return m_manager.GetInstitutionDetails(institutionId);
        }

        public CreateUserResponse CreateUser(string institutionId, User user)
        {
            throw new System.NotImplementedException();
        }

        public UserDetailsResponse GetUserDetails(string institutionId, string userId)
        {
            throw new System.NotImplementedException();
        }

        public TasksForAppResponse GetTasksForApplication(string applicationId)
        {
            throw new System.NotImplementedException();
        }

        public CreateTaskForAppResponse CreateTaskForApplication(AppTask apptask)
        {
            throw new System.NotImplementedException();
        }

        public CreateGroupResponse CreateGroup(Group group)
        {
            throw new System.NotImplementedException();
        }

        public SynchronizedObjectsResponse GetSynchronizedObjects(string groupId, string userId, string since)
        {
            throw new System.NotImplementedException();
        }

        public CreateSynchronizedObjectResponse CreateSynchronizedObject(string groupId, string userId, SynchronizedObject synchronizedObject)
        {
            throw new System.NotImplementedException();
        }
    }
}
