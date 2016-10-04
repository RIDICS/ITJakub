using System;
using System.Linq;
using Castle.Facilities.AutoTx;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ITJakub.ITJakubService.Core.Test
{
    [TestClass]
    [DeploymentItem("ITJakub.ITJakubService.Core.Test.Container.Config")]
    [DeploymentItem("log4net.config")]
    public class PermissionManagerTest
    {
        private PermissionManager m_permissionManager;
        private UserManager m_userManager;
        private MockPermissionRepository m_mockRepository;

        public PermissionManagerTest()
        {
            new AutoTxFacility(); // todo hack for deploying Unit test (copy dll to output directory)
        }

        [TestInitialize]
        public void Init()
        {
            var container = Container.Current;
            m_permissionManager = container.Resolve<PermissionManager>();
            m_userManager = container.Resolve<UserManager>();
            m_mockRepository = (MockPermissionRepository)container.Resolve<PermissionRepository>();
            m_mockRepository.IsAdmin = true;
        }

        [TestMethod]
        public void CreateUserTest()
        {
            var guid = Guid.NewGuid();
            var newUserContract = new PrivateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", guid),
                UserName = string.Format("user{0}", guid)
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void CreateGroupTest()
        {
            var newUserContract = new PrivateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var group = m_permissionManager.CreateGroup(groupName, "Just testing creating group");
            Assert.IsNotNull(group);
        }

        [TestMethod]
        public void AddMemberToGroupTest()
        {
            var newUserContract = new PrivateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var group = m_permissionManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new PrivateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new PrivateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var secondMember = m_userManager.CreateLocalUser(secondMemberContract);

            m_permissionManager.AddUserToGroup(firstMember.Id, group.Id);

            m_permissionManager.AddUserToGroup(secondMember.Id, group.Id);

            var groupDetails = m_permissionManager.GetGroupDetail(group.Id);

            Assert.IsNotNull(groupDetails);
            Assert.IsNotNull(groupDetails.Members);
            Assert.AreEqual(2, groupDetails.Members.Count);
        }

        [TestMethod]
        public void RemoveMemberFromGroupTest()
        {
            var newUserContract = new PrivateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var group = m_permissionManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new PrivateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new PrivateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var secondMember = m_userManager.CreateLocalUser(secondMemberContract);

            m_permissionManager.AddUserToGroup(firstMember.Id, group.Id);

            m_permissionManager.AddUserToGroup(secondMember.Id, group.Id);

            m_permissionManager.RemoveUserFromGroup(firstMember.Id, group.Id);

            var groupDetails = m_permissionManager.GetGroupDetail(group.Id);

            Assert.IsNotNull(groupDetails);
            Assert.IsNotNull(groupDetails.Members);
            Assert.AreEqual(1, groupDetails.Members.Count);
            Assert.AreEqual(secondMember.Id, groupDetails.Members.First().Id);
        }

        [TestMethod]
        public void GetUsersByGroupTest()
        {
            var newUserContract = new PrivateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var group = m_permissionManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new PrivateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new PrivateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var secondMember = m_userManager.CreateLocalUser(secondMemberContract);

            m_permissionManager.AddUserToGroup(firstMember.Id, group.Id);

            m_permissionManager.AddUserToGroup(secondMember.Id, group.Id);

            var users = m_permissionManager.GetUsersByGroup(group.Id);

            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count);
        }

        [TestMethod]
        public void GetGroupsByUserTest()
        {
            var newUserContract = new PrivateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var groupName = string.Format("TestGroup{0}", Guid.NewGuid());
            var groupName2 = string.Format("TestGroup{0}", Guid.NewGuid());
            var group = m_permissionManager.CreateGroup(groupName, "Just testing group with member");
            var group2 = m_permissionManager.CreateGroup(groupName2, "Just testing group with member");

            var firstMemberContract = new PrivateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new PrivateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = string.Format("second@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var secondMember = m_userManager.CreateLocalUser(secondMemberContract);

            m_permissionManager.AddUserToGroup(firstMember.Id, group.Id);

            m_permissionManager.AddUserToGroup(firstMember.Id, group2.Id);

            m_permissionManager.AddUserToGroup(secondMember.Id, group.Id);

            var groupsForFirstMember = m_permissionManager.GetGroupsByUser(firstMember.Id)
                .Where(x => x.Description != "Default user group");

            var groupsForSecondMember = m_permissionManager.GetGroupsByUser(secondMember.Id)
                .Where(x => x.Description != "Default user group");

            Assert.IsNotNull(groupsForFirstMember);
            Assert.AreEqual(2, groupsForFirstMember.Count());

            Assert.IsNotNull(groupsForSecondMember);
            Assert.AreEqual(1, groupsForSecondMember.Count());
        }
       
    }
}