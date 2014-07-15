using System.Collections.Generic;
using System.Net.Mail;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.RequestObjects;
using ITJakub.MobileApps.DataContracts.ResponseObjects;

namespace ITJakub.MobileApps.Core
{
    public class MobileServiceManager : IMobileAppsService
    {
        public string TestMethod(string test)
        {
            return string.Format("Hello {0}", test);
        }

        public CreateInstitutionResponse CreateInstitution(Institution institution)
        {

            return new CreateInstitutionResponse();
            throw new System.NotImplementedException();
        }

        public InstitutionDetailsResponse GetInstitutionDetails(string institutionId)
        {
            return new InstitutionDetailsResponse(){Name = "jmeno Institusce", Users = new List<UserDetailsResponse>(){new UserDetailsResponse(){Email = "mail",FirstName = "Pepa",LastName = "Novak", Role = "reditel"},new UserDetailsResponse(){Email = "email@cisu", FirstName = "Stana", LastName = "Novakova", Role = "teacher"}}};
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