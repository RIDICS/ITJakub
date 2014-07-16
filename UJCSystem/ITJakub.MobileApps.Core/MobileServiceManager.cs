using System.Collections.Generic;
using System.Net.Mail;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core
{
    public class MobileServiceManager : IMobileAppsService
    {
        public string TestMethod(string test)
        {
            return string.Format("Hello {0}", test);
        }

        public void CreateInstitution(Institution institution)
        {
        }

        public Institution GetInstitutionDetails(string institutionId)
        {
            return new Institution(){Name = "jmeno Institusce", Principal = new User(){Email = "mail",FirstName = "Pepa",LastName = "Novak"}};
        }

        public void CreateUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public UserDetails GetUserDetails(string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AppTaskDetails> GetTasksForApplication(string applicationId)
        {
            throw new System.NotImplementedException();
        }

        public void CreateTaskForApplication(string applicationId, AppTask apptask)
        {
            throw new System.NotImplementedException();
        }

        public void CreateGroup(string institutionId, Group @group)
        {
            throw new System.NotImplementedException();
        }

        public Group GetGroupDetails(string institutionId, string groupId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string userId, string since)
        {
            throw new System.NotImplementedException();
        }

        public void CreateSynchronizedObject(string groupId, string userId, SynchronizedObject synchronizedObject)
        {
            throw new System.NotImplementedException();
        }
    }
}