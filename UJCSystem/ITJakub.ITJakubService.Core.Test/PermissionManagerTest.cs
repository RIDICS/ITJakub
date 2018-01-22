using System;
using System.Linq;
using Castle.Facilities.AutoTx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.ITJakubService.Core.Test
{
    [TestClass]
    [DeploymentItem("ITJakub.ITJakubService.Core.Test.Container.Config")]
    [DeploymentItem("log4net.config")]
    public class PermissionManagerTest
    {
        private UserManager m_userManager;
        private MockPermissionRepository m_mockRepository;
        private UserGroupManager m_userGroupManager;

        public PermissionManagerTest()
        {
            new AutoTxFacility(); // todo hack for deploying Unit test (copy dll to output directory)
        }

        [TestInitialize]
        public void Init()
        {
            var container = Container.Current;
            m_userManager = container.Resolve<UserManager>();
            m_userGroupManager = container.Resolve<UserGroupManager>();
            m_mockRepository = (MockPermissionRepository)container.Resolve<PermissionRepository>();
            m_mockRepository.IsAdmin = true;
        }

        [TestMethod]
        public void CreateUserTest()
        {
            var guid = Guid.NewGuid();
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", guid),
                NewPassword = "123456",
                UserName = string.Format("user{0}", guid)
            };

            var user = m_userManager.CreateNewUser(newUserContract);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void CreateGroupTest()
        {
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateNewUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var group = m_userGroupManager.CreateGroup(groupName, "Just testing creating group");
            Assert.IsNotNull(group);
        }

        [TestMethod]
        public void AddMemberToGroupTest()
        {
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateNewUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var secondMemberId = m_userManager.CreateNewUser(secondMemberContract);

            m_userGroupManager.AddUserToGroup(firstMemberId, groupId);

            m_userGroupManager.AddUserToGroup(secondMemberId, groupId);

            var groupDetails = m_userGroupManager.GetGroupDetail(groupId);

            Assert.IsNotNull(groupDetails);
            Assert.IsNotNull(groupDetails.Members);
            Assert.AreEqual(2, groupDetails.Members.Count);
        }

        [TestMethod]
        public void RemoveMemberFromGroupTest()
        {
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateNewUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var secondMemberId = m_userManager.CreateNewUser(secondMemberContract);

            m_userGroupManager.AddUserToGroup(firstMemberId, groupId);

            m_userGroupManager.AddUserToGroup(secondMemberId, groupId);

            m_userGroupManager.RemoveUserFromGroup(firstMemberId, groupId);

            var groupDetails = m_userGroupManager.GetGroupDetail(groupId);

            Assert.IsNotNull(groupDetails);
            Assert.IsNotNull(groupDetails.Members);
            Assert.AreEqual(1, groupDetails.Members.Count);
            Assert.AreEqual(secondMemberId, groupDetails.Members.First().Id);
        }

        [TestMethod]
        public void GetUsersByGroupTest()
        {
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateNewUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var secondMemberId = m_userManager.CreateNewUser(secondMemberContract);

            m_userGroupManager.AddUserToGroup(firstMemberId, groupId);

            m_userGroupManager.AddUserToGroup(secondMemberId, groupId);

            var users = m_userGroupManager.GetUsersByGroup(groupId);

            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count);
        }

        [TestMethod]
        public void GetGroupsByUserTest()
        {
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateNewUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var groupName2 = string.Format("TestGroup{0}", Guid.NewGuid());
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");
            var group2Id = m_userGroupManager.CreateGroup(groupName2, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var secondMemberId = m_userManager.CreateNewUser(secondMemberContract);

            m_userGroupManager.AddUserToGroup(firstMemberId, groupId);

            m_userGroupManager.AddUserToGroup(firstMemberId, group2Id);

            m_userGroupManager.AddUserToGroup(secondMemberId, groupId);

            var groupsForFirstMember = m_userGroupManager.GetGroupsByUser(firstMemberId)
                .Where(x => x.Description != "Default user group");

            var groupsForSecondMember = m_userGroupManager.GetGroupsByUser(secondMemberId)
                .Where(x => x.Description != "Default user group");

            Assert.IsNotNull(groupsForFirstMember);
            Assert.AreEqual(2, groupsForFirstMember.Count());

            Assert.IsNotNull(groupsForSecondMember);
            Assert.AreEqual(1, groupsForSecondMember.Count());
        }
       
    }
}