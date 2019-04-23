using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.DataEntities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Test.Containers;
using Vokabular.MainService.Test.LiveMock;
using Vokabular.MainService.Test.Mock;

namespace Vokabular.MainService.Test
{
    [TestClass]
    public class PermissionManagerTest
    {
        private UserManager m_userManager;
        private MockPermissionRepository m_mockRepository;
        private UserGroupManager m_userGroupManager;

        [TestInitialize]
        public void Init()
        {
            var container = new DryIocContainer();
            container.Install<MainServiceCoreContainerRegistration>();

            var services = new ServiceCollection();
            services.AddDataEntitiesServices();
            container.Populate(services);

            container.AddPerWebRequest<IHttpContextAccessor, MockHttpContextAccessor>();
            container.ReplacePerWebRequest<PermissionRepository, MockPermissionRepository>();
            container.ReplacePerWebRequest<UserRepository, MockUserRepository>();
            container.ReplacePerWebRequest<ICommunicationTokenProvider, MockCommunicationTokenProvider>();
            container.ReplacePerWebRequest<ICommunicationTokenGenerator, MockCommunicationTokenGenerator>();
            
            container.InitAutoMapper();
            container.InitNHibernate();

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
                Email = $"test@{guid}.test",
                NewPassword = "123456",
                UserName = $"user{guid}"
            };

            var userId = m_userManager.CreateNewUser(newUserContract);
            Assert.IsNotNull(userId);
            Assert.AreNotEqual(0, userId);
        }

        [TestMethod]
        public void CreateGroupTest()
        {
            var guid = Guid.NewGuid();
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test@{guid}.test",
                NewPassword = "123456",
                UserName = guid.ToString()
            };

            m_userManager.CreateNewUser(newUserContract);
            var groupName = $"TestGroup{Guid.NewGuid()}";
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing creating group");
            Assert.IsNotNull(groupId);
            Assert.AreNotEqual(0, groupId);
        }

        [TestMethod]
        public void AddMemberToGroupTest()
        {
            var guid = Guid.NewGuid();
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test@{guid}.test",
                NewPassword = "123456",
                UserName = guid.ToString()
            };

            m_userManager.CreateNewUser(newUserContract);
            var groupName = $"TestGroup{Guid.NewGuid()}";
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = $"first@{Guid.NewGuid()}.member",
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = $"second@{Guid.NewGuid()}.member",
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
            var guid = Guid.NewGuid();
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test@{guid}.test",
                NewPassword = "123456",
                UserName = guid.ToString()
            };

            m_userManager.CreateNewUser(newUserContract);
            var groupName = $"TestGroup{Guid.NewGuid()}";
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = $"first@{Guid.NewGuid()}.member",
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = $"second@{Guid.NewGuid()}.member",
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
            var guid = Guid.NewGuid();
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test@{guid}.test",
                NewPassword = "123456",
                UserName = guid.ToString()
            };

            m_userManager.CreateNewUser(newUserContract);
            var groupName = $"TestGroup{Guid.NewGuid()}";
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = $"first@{Guid.NewGuid()}.member",
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = $"second@{Guid.NewGuid()}.member",
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
            var guid = Guid.NewGuid();
            var newUserContract = new CreateUserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test@{guid}.test",
                NewPassword = "123456",
                UserName = guid.ToString()
            };

            m_userManager.CreateNewUser(newUserContract);
            var groupName = $"TestGroup{Guid.NewGuid()}";
            var groupName2 = $"TestGroup{Guid.NewGuid()}";
            var groupId = m_userGroupManager.CreateGroup(groupName, "Just testing group with member");
            var group2Id = m_userGroupManager.CreateGroup(groupName2, "Just testing group with member");

            var firstMemberContract = new CreateUserContract
            {
                FirstName = "First",
                LastName = "Member",
                Email = $"first@{Guid.NewGuid()}.member",
                NewPassword = "123456",
                UserName = Guid.NewGuid().ToString()
            };

            var firstMemberId = m_userManager.CreateNewUser(firstMemberContract);

            var secondMemberContract = new CreateUserContract
            {
                FirstName = "Second",
                LastName = "Member",
                Email = $"second@{Guid.NewGuid()}.member",
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