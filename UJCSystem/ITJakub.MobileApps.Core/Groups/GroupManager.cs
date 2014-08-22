using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataEntities;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;
using log4net;

namespace ITJakub.MobileApps.Core.Groups
{
    public class GroupManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly EnterCodeGenerator m_enterCodeGenerator;
        private readonly UsersRepository m_usersRepository;


        public GroupManager(UsersRepository usersRepository, EnterCodeGenerator enterCodeGenerator, int maxAttemptsToSave)
        {
            MaxAttemptsToSave = maxAttemptsToSave;
            m_usersRepository = usersRepository;
            m_enterCodeGenerator = enterCodeGenerator;
        }

        private int MaxAttemptsToSave { get; set; }

        public UserGroupsContract GetGroupByUser(long userId)
        {
            User user = m_usersRepository.GetUserWithGroups(userId);

            var result = new UserGroupsContract
            {
                MemberOfGroup = Mapper.Map<IEnumerable<Group>, List<GroupDetailContract>>(user.MemberOfGroups),
                OwnedGroups = Mapper.Map<IEnumerable<Group>, List<OwnedDetailGroupContract>>(user.CreatedGroups)
            };

            return result;
        }

        public CreateGroupResponse CreateGroup(long userId, string groupName)
        {
            User user = m_usersRepository.Load<User>(userId);
            
            var group = new Group {Author = user, CreateTime = DateTime.UtcNow, Name = groupName, IsActive = true};

            int attempt = 0;
            while (attempt < MaxAttemptsToSave)
            {
                try
                {
                    group.EnterCode = m_enterCodeGenerator.GenerateCode();
                    m_usersRepository.Create(group);
                    return new CreateGroupResponse {EnterCode = group.EnterCode};
                }
                catch (CreateEntityFailedException ex)
                {
                    ++attempt;

                    if (m_log.IsWarnEnabled)
                        m_log.WarnFormat("Could not save Group to Database. AttemptCount: '{0}', EntryCode: '{1}'. Exception: '{2}'", attempt,
                            group.EnterCode, ex);
                }
            }

            if (m_log.IsErrorEnabled)
                m_log.ErrorFormat("Cannot create group, Maximum attemts to save exceeded");

            return null;
        }

        public void AddUserToGroup(string groupAccessCode, long userId)
        {
            Group group = m_usersRepository.FindByEnterCode(groupAccessCode);
            var user = m_usersRepository.Load<User>(userId);
            group.Members.Add(user);
            m_usersRepository.Update(group);
        }

        public GroupDetailContract GetGroupDetails(long groupId)
        {
            var group = m_usersRepository.GetGroupDetails(groupId);
            var groupContract = Mapper.Map<Group, GroupDetailContract>(group);
            return groupContract;
        }

        public IList<GroupMemberContract> GetGroupMembers(long groupId)
        {
            var group = m_usersRepository.GetGroupDetails(groupId);
            return Mapper.Map<IList<User>, IList<GroupMemberContract>>(group.Members);
        }

        public IList<long> GetGroupMemberIds(long groupId)
        {
            var group = m_usersRepository.GetGroupDetails(groupId);
            return group.Members.Select(user => user.Id).ToList();
        }

        public IList<GroupDetailsUpdateContract> GetGroupsUpdate(IList<OldGroupDetailsContract> oldGroups)
        {
            var groupsInfo = m_usersRepository.GetGroupMembers(oldGroups.Select(contract => contract.Id));
            var groupList = Mapper.Map<IList<Group>, IList<GroupDetailsUpdateContract>>(groupsInfo);

            foreach (var groupInfo in groupList)
            {
                var groupId = groupInfo.Id;
                var oldGroupInfo = oldGroups.Single(group => group.Id == groupId);
                groupInfo.Members =
                    groupInfo.Members.Where(member => !oldGroupInfo.MemberIds.Contains(member.Id))
                        .ToList();
            }

            return groupList.Where(group => group.Members.Count > 0).ToList();
        }

        public void AssignTaskToGroup(long groupId, long taskId)
        {
            var task = m_usersRepository.Load<Task>(taskId);
            var group = m_usersRepository.FindById<Group>(groupId);
            group.Task = task;

            m_usersRepository.Update(group);
        }
    }
}