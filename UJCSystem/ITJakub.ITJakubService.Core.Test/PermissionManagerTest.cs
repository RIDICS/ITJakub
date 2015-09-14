using System;
using Castle.Windsor;
using ITJakub.Shared.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq.Clauses.ResultOperators;

namespace ITJakub.ITJakubService.Core.Test
{
    [TestClass]
    public class PermissionManagerTest
    {
        private readonly WindsorContainer m_container = Container.Current;

        private readonly PermissionManager m_permissionManager;
        private readonly UserManager m_userManager;

        public PermissionManagerTest()
        {
            m_permissionManager = m_container.Resolve<PermissionManager>();
            m_userManager = m_container.Resolve<UserManager>();
        }

        [TestMethod]
        public void CreateGroupTest()
        {
            var newUserContract = new UserContract
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.test",
                UserName = Guid.NewGuid().ToString()
            };

            var user = m_userManager.CreateLocalUser(newUserContract);
            var group = m_permissionManager.CreateGroup(user.Id, "TestGroup", "Just testing creating group");
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
            var group = m_permissionManager.CreateGroup(user.Id, "TestGroup", "Just testing group with member");

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
    }
}
