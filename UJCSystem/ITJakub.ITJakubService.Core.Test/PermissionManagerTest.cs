using System;
using System.Linq;
using Castle.Windsor;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.Shared.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ITJakub.ITJakubService.Core.Test
{
    [TestClass]
    public class PermissionManagerTest
    {
        private readonly WindsorContainer m_container = Container.Current;
        private readonly PermissionManager m_permissionManager;
        private readonly UserManager m_userManager;
        private readonly string testUserUsername = "testUser";

        public PermissionManagerTest()
        {
            m_permissionManager = m_container.Resolve<PermissionManager>();
            m_userManager = m_container.Resolve<UserManager>();
        }

        [TestMethod]
        public void CreateUserTest()
        {
            //var username = Guid.NewGuid().ToString();
            var newUserContract = new UserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = testUserUsername
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void CreateGroupTest()
        {
            var newUserContract = new UserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var group = m_permissionManager.CreateGroup("TestGroup", "Just testing creating group");
            Assert.IsNotNull(group);
        }

        [TestMethod]
        public void AddMemberToGroupTest()
        {
            var newUserContract = new UserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var group = m_permissionManager.CreateGroup("TestGroup", "Just testing group with member");

            var firstMemberContract = new UserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new UserContract
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
            var newUserContract = new UserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var group = m_permissionManager.CreateGroup("TestGroup", "Just testing group with member");

            var firstMemberContract = new UserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new UserContract
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
            var newUserContract = new UserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var group = m_permissionManager.CreateGroup("TestGroup", "Just testing group with member");

            var firstMemberContract = new UserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new UserContract
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
            var newUserContract = new UserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = string.Format("test@{0}.test", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var group = m_permissionManager.CreateGroup("TestGroup", "Just testing group with member");
            var group2 = m_permissionManager.CreateGroup("TestGroup2", "Just testing group with member");

            var firstMemberContract = new UserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = string.Format("first@{0}.member", Guid.NewGuid()),
                UserName = Guid.NewGuid().ToString()
            };

            var firstMember = m_userManager.CreateLocalUser(firstMemberContract);

            var secondMemberContract = new UserContract
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

            var groupsForFirstMember = m_permissionManager.GetGroupsByUser(firstMember.Id);

            var groupsForSecondMember = m_permissionManager.GetGroupsByUser(secondMember.Id);

            Assert.IsNotNull(groupsForFirstMember);
            Assert.AreEqual(2, groupsForFirstMember.Count);

            Assert.IsNotNull(groupsForSecondMember);
            Assert.AreEqual(1, groupsForSecondMember.Count);
        }
       
    }
}